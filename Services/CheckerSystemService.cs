using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContestSystemDbStructure.Models;
using ContestSystem.Models.Misc;
using Microsoft.Extensions.Configuration;

namespace ContestSystem.Services
{
    public class CheckerSystemService
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> _checkerServers = new List<string>();
        private int _currentServerIndex = -1;
        private readonly Dictionary<long, int> _serverForSolution = new Dictionary<long, int>();
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly int _localPort = 6500;

        public CheckerSystemService(IConfiguration configuration)
        {
            _configuration = configuration;
            IConfigurationSection checkerServersSection = _configuration.GetSection("CheckerServers");
            List<string> checkerServersFromConf = checkerServersSection.AsEnumerable()
                .Select(keyValPair => keyValPair.Value).Skip(1).ToList();
            foreach (var server in checkerServersFromConf)
            {
                if (PingServer(server))
                {
                    _checkerServers.Add(server);
                }
            }

            if (_checkerServers.Count > 0)
            {
                _currentServerIndex = 0;
            }

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Получение списка компиляторов, поддержка которых есть НА ВСЕХ серверах
        public async Task<IEnumerable<CompilerInfo>> GetAvailableCompilersAsync()
        {
            List<CompilerInfo> finalCompilers = new List<CompilerInfo>();
            List<List<CompilerInfo>> compilersLists = new List<List<CompilerInfo>>();
            for (int i = 0; i < _checkerServers.Count; i++)
            {
                string server = _checkerServers[i];
                if (await PingServerAsync(server))
                {
                    var response = await _httpClient.GetAsync($"http://{server}/api/compiler");
                    compilersLists.Add((List<CompilerInfo>)await response.Content.ReadFromJsonAsync<IEnumerable<CompilerInfo>>());
                }
            }

            if (compilersLists.Count == 0)
            {
                var response = await _httpClient.GetAsync($"http://localhost:{_localPort}/api/compiler");
                return await response.Content.ReadFromJsonAsync<IEnumerable<CompilerInfo>>();
            }

            finalCompilers.AddRange(compilersLists[0]);
            foreach (var list in compilersLists)
            {
                finalCompilers = (List<CompilerInfo>)finalCompilers.Intersect(list);
            }

            return finalCompilers;
        }

        // Отправка чекера задачи на все сервера для компиляции (после одобрения глобальным модератором)
        public async Task<Checker> SendCheckerForCompilationAsync(Checker checker)
        {
            bool firstCompilationDone = false;
            HttpContent content;
            HttpResponseMessage response;
            Checker resultedChecker;
            for (int i = 0; i < _checkerServers.Count; i++)
            {
                string server = _checkerServers[i];
                if (await PingServerAsync(server))
                {
                    content = JsonContent.Create(checker);
                    response = await _httpClient.PostAsync($"http://{server}/api/checker", content);
                    if (!firstCompilationDone)
                    {
                        resultedChecker = await response.Content.ReadFromJsonAsync<Checker>();
                        firstCompilationDone = true;
                    }
                }
            }

            content = JsonContent.Create(checker);
            response = await _httpClient.PostAsync($"http://localhost:{_localPort}/api/checker", content);
            resultedChecker = await response.Content.ReadFromJsonAsync<Checker>();
            return resultedChecker;
        }

        // Отправка решения на компиляцию (ДО прогона по тестам)
        public async Task<Solution> CompileSolutionAsync(Solution solution)
        {
            string server = await GetServerForSolutionAsync(solution.Id);
            var content = JsonContent.Create(solution);
            var response = await _httpClient.PostAsync($"http://{server}/api/solution", content);
            return await response.Content.ReadFromJsonAsync<Solution>();
        }

        // Отправка решения на проверку
        public async Task<TestResult> RunTestForSolutionAsync(Solution solution, short testNumber)
        {
            string server = await GetServerForSolutionAsync(solution.Id);
            var content = JsonContent.Create(solution);
            var response = await _httpClient.PostAsync($"http://{server}/api/test?testNumber={testNumber}", content);
            return await response.Content.ReadFromJsonAsync<TestResult>();
        }

        // Выбор сервера проверки по приницпу Round-Robin
        private async Task<string> GetServerForSolutionAsync(long solutionId)
        {
            int serverIndex;
            if (_serverForSolution.ContainsKey(solutionId))
            {
                serverIndex = _serverForSolution[solutionId];
                int index = serverIndex;
                do
                {
                    if (await PingServerAsync(_checkerServers[index]))
                    {
                        return _checkerServers[serverIndex];
                    }

                    index = (index + 1) % _checkerServers.Count;
                } while (index != serverIndex);
            }
            else
            {
                int index = (_currentServerIndex + 1) % _checkerServers.Count;
                do
                {
                    if (await PingServerAsync(_checkerServers[index]))
                    {
                        _currentServerIndex = index;
                        _serverForSolution.Add(solutionId, _currentServerIndex);
                        return _checkerServers[_currentServerIndex];
                    }

                    index = (index + 1) % _checkerServers.Count;
                } while (index != _currentServerIndex);
            }

            return $"localhost:{_localPort}";
        }

        // Синхронный пинг
        private bool PingServer(string server)
        {
            string hostnameOrAddress = ParseHostnameOrAddressWithoutPort(server);
            PingOptions pingOptions = new PingOptions(128, true);
            Ping ping = new Ping();
            byte[] buffer = new byte[32];
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    PingReply pingReply = ping.Send(hostnameOrAddress, 2000, buffer, pingOptions);
                    if (!(pingReply == null))
                    {
                        if (pingReply.Status == IPStatus.Success)
                        {
                            return true;
                        }
                    }
                }
                catch (PingException)
                {
                    return false;
                }
                catch (SocketException)
                {
                    return false;
                }
            }

            return false;
        }

        // Асинхронный пинг
        private async Task<bool> PingServerAsync(string server)
        {
            string hostnameOrAddress = ParseHostnameOrAddressWithoutPort(server);
            PingOptions pingOptions = new PingOptions(128, true);
            Ping ping = new Ping();
            byte[] buffer = new byte[32];
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    PingReply pingReply = await ping.SendPingAsync(hostnameOrAddress, 2000, buffer, pingOptions);
                    if (!(pingReply == null))
                    {
                        if (pingReply.Status == IPStatus.Success)
                        {
                            return true;
                        }
                    }
                }
                catch (PingException)
                {
                    return false;
                }
                catch (SocketException)
                {
                    return false;
                }
            }

            return false;
        }

        // Получение адреса или FQDN без номера порта(так как сервер может быть задан с портом, например localhost:1337)
        private string ParseHostnameOrAddressWithoutPort(string server)
        {
            Regex regexWithPort = new Regex(@"(\w)*\:[0-9]*");
            if (regexWithPort.IsMatch(server))
            {
                return server.Substring(0, server.LastIndexOf(':'));
            }

            return server;
        }
    }
}