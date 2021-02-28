using CheckerSystemBaseStructures;
using ContestSystem.Models.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Services
{
    public class CheckerSystemService
    {
        private readonly string checkerSystemAddress = "http://localhost:50005/";
        private FilesStorageService _fs;

        public CheckerSystemService(FilesStorageService fs)
        {
            _fs = fs;
        }

        public async Task<byte[]> GetProblemArchiveAsync(long problemId)
        {
            throw new NotImplementedException();
        }

        public async Task<FilesCheckResult> UploadProblemArchiveAsync(long problemId, string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetCompilersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<VerdictModel> SendSolutionAsync(long problemId, int compilerId, long solutionId, string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<VerdictModel> RunTestAsync(long problemId, long solutionId, int testNumber)
        {
            throw new NotImplementedException();
        }

        private string FormFullRequest(string request)
        {
            throw new NotImplementedException();
        }
    }
}
