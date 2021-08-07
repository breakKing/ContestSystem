using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ContestSystemDbStructure.Models;
using ContestSystem.Models.Misc;
using System;
using Microsoft.Extensions.Logging;
using ContestSystem.Models.DbContexts;
using Microsoft.EntityFrameworkCore;
using ContestSystemDbStructure.Enums;

namespace ContestSystem.Services
{
    public class CheckerSystemService
    {
        private readonly ILogger<CheckerSystemService> _logger;
        private readonly HttpClient _httpClient = new HttpClient();

        private readonly int _localPort = 6500;
        private readonly short _minutesFromLastCompilersUpdate = 5;
        private readonly string _protocol = "http";

        private bool serversAreInited = false;

        public CheckerSystemService(ILogger<CheckerSystemService> logger)
        {
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Получение списка компиляторов, поддержка которых есть на серверах
        public async Task<List<CompilerInfo>> GetAvailableCompilersAsync(MainDbContext dbContext)
        {
            await InitServersIfNeededAsync(dbContext);
            List<CompilerInfo> finalCompilers = new List<CompilerInfo>();
            var servers = await dbContext.CheckerServers.ToListAsync();
            foreach (var server in servers)
            {
                await UpdateServerCompilerAsync(dbContext, server);
            }
            servers = await dbContext.CheckerServers.ToListAsync();
            foreach (var server in servers)
            {
                var compilers = server.CheckerServerCompilers.ConvertAll(csc => new CompilerInfo
                {
                    GUID = csc.CompilerGUID,
                    Name = csc.CompilerName
                });
                foreach (var cmp in compilers)
                {
                    if (finalCompilers.FirstOrDefault(c => c.GUID == cmp.GUID) == null)
                    {
                        finalCompilers.Add(cmp);
                    }
                }
            }
            return finalCompilers.Count > 0 ? finalCompilers : null;
        }

        // Отправка чекера задачи на все сервера для компиляции (после одобрения глобальным модератором)
        public async Task<Checker> SendCheckerForCompilationAsync(MainDbContext dbContext, Checker checker)
        {
            await InitServersIfNeededAsync(dbContext);
            _logger.LogInformation($"Отправлена на компиляцию сущность \"Checker\" с идентификатором {checker.Id}");
            Checker resultedChecker = null;
            var servers = await dbContext.CheckerServers.ToListAsync();
            foreach (var server in servers)
            {
                if (await CheckServerConnectionAsync(server.Address))
                {
                    var compiledChecker = await PostRequestAsync<Checker, Checker>(server.Address, "api/checker", checker);
                    if (compiledChecker == null)
                    {
                        _logger.LogWarning($"При компиляции чекера с идентификатором {checker.Id} на сервере проверки {server.Address} не было получено ответа");
                    }
                    else
                    {
                        if (compiledChecker.CompilationVerdict != VerdictType.CompilationSucceed)
                        {
                            _logger.LogInformation($"В результате компиляции \"Checker\" с идентификатором {checker.Id} на сервере проверки {server.Address} возникли ошибки");
                            resultedChecker ??= compiledChecker;
                        }
                        else
                        {
                            _logger.LogInformation($"Компиляция \"Checker\" с идентификатором {checker.Id} на сервере проверки {server.Address} прошла успешно");
                            resultedChecker = compiledChecker;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"Неудачная попытка отправки чекера с идентификатором {checker.Id} на компиляцию на сервер проверки {server.Address} в связи с недоступностью сервера");
                }
            }
            return resultedChecker;
        }

        // Отправка решения на компиляцию (ДО прогона по тестам)
        public async Task<Solution> CompileSolutionAsync(MainDbContext dbContext, Solution solution)
        {
            await InitServersIfNeededAsync(dbContext);
            var server = await GetServerForSolutionAsync(dbContext, solution);
            if (server == null)
            {
                _logger.LogWarning($"Для компиляции решения с идентификатором {solution.Id} не нашлось ни одного доступного сервера проверки");
                return null;
            }
            return await PostRequestAsync<Solution, Solution>(server.Address, "api/solution", solution);
        }

        // Отправка решения на проверку
        public async Task<TestResult> RunTestForSolutionAsync(MainDbContext dbContext, Solution solution, short testNumber)
        {
            await InitServersIfNeededAsync(dbContext);
            var server = await GetServerForSolutionAsync(dbContext, solution);
            if (server == null)
            {
                _logger.LogWarning($"Для проверки решения с идентификатором {solution.Id} на тесте {testNumber} не нашлось ни одного доступного сервера проверки");
                return null;
            }
            return await PostRequestAsync<Solution, TestResult>(server.Address, $"api/test?testNumber={testNumber}", solution);
        }

        // Инициализация всех серверов
        private async Task InitServersIfNeededAsync(MainDbContext dbContext)
        {
            if (serversAreInited)
            {
                return;
            }
            var servers = await dbContext.CheckerServers.ToListAsync();
            if (servers == null || servers.Count == 0)
            {
                await AddLocalhostCheckerServerAsync(dbContext);
                servers = await dbContext.CheckerServers.ToListAsync();
            }
            else
            {
                if (!servers.Any(s => s.Address == $"localhost:{_localPort}"))
                {
                    await AddLocalhostCheckerServerAsync(dbContext);
                    servers = await dbContext.CheckerServers.ToListAsync();
                }
                foreach (var server in servers)
                {
                    await UpdateServerCompilerAsync(dbContext, server);
                }
            }
            serversAreInited = true;
        }

        // Добавление локального сервера (используемого в последнюю очередь)
        private async Task AddLocalhostCheckerServerAsync(MainDbContext dbContext)
        {
            var localhostServer = new CheckerServer
            {
                Address = $"localhost:{_localPort}",
                LastTimeUsedForSolutionUTC = DateTime.UtcNow,
                LastTimeCompilersUpdatedUTC = DateTime.UtcNow.AddMinutes(-_minutesFromLastCompilersUpdate)
            };
            await dbContext.CheckerServers.AddAsync(localhostServer);
            await dbContext.SaveChangesAsync();
        }

        // Запрос и обновление списка компиляторов на сервере (асинхронно)
        private async Task UpdateServerCompilerAsync(MainDbContext dbContext, CheckerServer server)
        {
            if (server != null)
            {
                if (server.LastTimeCompilersUpdatedUTC.AddMinutes(_minutesFromLastCompilersUpdate) <= DateTime.UtcNow)
                {
                    if (await CheckServerConnectionAsync(server.Address))
                    {
                        var compilersUpToDate = (await GetRequestAsync<IEnumerable<CompilerInfo>>(server.Address, "api/compiler"))?.ToList() ?? new List<CompilerInfo>();
                        var compilers = await dbContext.CheckerServersCompilers.Where(csc => csc.CheckerServerId == server.Id).ToListAsync();
                        if (compilersUpToDate == null || compilersUpToDate.Count == 0)
                        {
                            if (compilers != null && compilers.Count > 0)
                            {
                                dbContext.CheckerServersCompilers.RemoveRange(compilers);
                            }
                        }
                        else
                        {
                            if (compilers == null || compilers.Count == 0)
                            {
                                await dbContext.CheckerServersCompilers.AddRangeAsync(compilersUpToDate.ConvertAll(c => new CheckerServerCompiler
                                {
                                    CheckerServerId = server.Id,
                                    CompilerGUID = c.GUID,
                                    CompilerName = c.Name,
                                }));
                            }
                            else
                            {
                                foreach (var cmp in compilersUpToDate)
                                {
                                    int index = compilers.FindIndex(c => c.CompilerGUID == cmp.GUID);
                                    if (index == -1)
                                    {
                                        await dbContext.CheckerServersCompilers.AddAsync(new CheckerServerCompiler
                                        {
                                            CheckerServerId = server.Id,
                                            CompilerGUID = cmp.GUID,
                                            CompilerName = cmp.Name
                                        });
                                    }
                                    else
                                    {
                                        compilers[index].CompilerName = cmp.Name;
                                        dbContext.CheckerServersCompilers.Update(compilers[index]);
                                    }
                                }
                            }
                        }
                        var serverFromDb = await dbContext.CheckerServers.FirstOrDefaultAsync(s => s.Id == server.Id);
                        serverFromDb.LastTimeCompilersUpdatedUTC = DateTime.UtcNow;
                        dbContext.CheckerServers.Update(serverFromDb);
                        try
                        {
                            await dbContext.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            await UpdateServerCompilerAsync(dbContext, server);
                        }
                    }
                }
            }
        }

        // Выбор сервера проверки по приницпу Round-Robin
        private async Task<CheckerServer> GetServerForSolutionAsync(MainDbContext dbContext, Solution solution)
        {
            if (solution == null)
            {
                return null;
            }
            var servers = await dbContext.CheckerServers.Where(s => s.CheckerServerCompilers.Any(csc => csc.CompilerGUID == solution.CompilerGUID))
                                                        .OrderBy(s => s.LastTimeUsedForSolutionUTC)
                                                        .ThenBy(s => s.Address != $"localhost:{_localPort}")
                                                        .ToListAsync();
            if (servers == null || servers.Count == 0)
            {
                return null;
            }
            if (solution.CheckerServerId != null)
            {
                var server = servers.FirstOrDefault(s => s.Id == solution.CheckerServerId);
                if (server != null && await CheckServerConnectionAsync(server.Address))
                {
                    return server;
                }
            }
            CheckerServer serverForSolution = null;
            foreach (var server in servers)
            {
                if (await CheckServerConnectionAsync(server.Address))
                {
                    solution.CheckerServerId = server.Id;
                    server.LastTimeUsedForSolutionUTC = DateTime.UtcNow;
                    dbContext.Solutions.Update(solution);
                    dbContext.CheckerServers.Update(server);
                    serverForSolution = server;
                    break;
                }
            }
            if (serverForSolution != null)
            {
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return await GetServerForSolutionAsync(dbContext, solution);
                }
            }
            return serverForSolution;
        }

        // Проверка связи с сервером проверки (асинхронно)
        private async Task<bool> CheckServerConnectionAsync(string server)
        {
            bool result = false;
            try
            {
                result = await PostRequestAsync<bool?, bool>(server, "api/connection", null);
            }
            catch { }
            return result;
        }

        // GET-запрос
        private async Task<TResponse> GetRequestAsync<TResponse>(string serverAddress, string requestUri)
        {
            TResponse result = default;
            string fullUri = $"{_protocol}://{serverAddress}/{requestUri}";
            try
            {
                var response = await _httpClient.GetAsync(fullUri);
                result = await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"При GET-запросе \"{fullUri}\" произошла ошибка");
                _logger.LogTrace(ex.StackTrace);
            }
            return result;
        }

        // POST-запрос
        private async Task<TResponse> PostRequestAsync<TRequest, TResponse>(string serverAddress, string requestUri, TRequest data)
        {
            TResponse result = default;
            string fullUri = $"{_protocol}://{serverAddress}/{requestUri}";
            var jsonData = JsonContent.Create(data);
            try
            {
                var response = await _httpClient.PostAsync(fullUri, jsonData);
                result = await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"При POST-запросе \"{fullUri}\" произошла ошибка");
                _logger.LogTrace(ex.StackTrace);
            }
            return result;
        }
    }
}