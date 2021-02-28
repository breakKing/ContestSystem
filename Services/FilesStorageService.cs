using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Services
{
    public class FilesStorageService
    {
        private readonly string rootPath = "Storage/";

        public FilesStorageService()
        {
            rootPath = Path.Combine(Directory.GetCurrentDirectory(), rootPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }
    }
}
