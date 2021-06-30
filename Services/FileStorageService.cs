using ContestSystem.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContestSystem.Services
{
    public class FileStorageService
    {
        private readonly string _storageDirectory = @"Storage";
        private readonly string _imagesDirectory = @"Images";
        private readonly string _contestsDirectory = @"Contests";
        private readonly string _postsDirectory = @"Posts";
        private readonly ILogger<FileStorageService> _logger;

        private string StorageDirectory => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _storageDirectory));
        private string ImagesDirectory => Path.GetFullPath(Path.Combine(StorageDirectory, _imagesDirectory));
        private string ContestsImagesDirectory => Path.GetFullPath(Path.Combine(ImagesDirectory, _contestsDirectory));
        private string PostsImagesDirectory => Path.GetFullPath(Path.Combine(ImagesDirectory, _postsDirectory));

        private List<string> AllowedImageTypes => new List<string>
        {
            "jpg",
            "jpeg",
            "png",
            "bmp",
            "tiff"
        };

        public FileStorageService(ILogger<FileStorageService> logger)
        {
            EnsureDirectoryCreated(StorageDirectory);
            EnsureDirectoryCreated(ImagesDirectory);
            EnsureDirectoryCreated(ContestsImagesDirectory);
            EnsureDirectoryCreated(PostsImagesDirectory);
            _logger = logger;
        }

        private void EnsureDirectoryCreated(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public async Task<string> SaveContestImageAsync(long contestId, IFormFile formFileForImage)
        {
            string result = "";
            if (formFileForImage != null)
            {
                string imageType = formFileForImage.FileName.Substring(formFileForImage.FileName.LastIndexOf('.') + 1).ToLower();
                if (AllowedImageTypes.Contains(imageType))
                {
                    string imagePath = Path.Combine(ContestsImagesDirectory, $"{contestId}.{imageType}");
                    try
                    {
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await formFileForImage.CopyToAsync(fileStream);
                        }
                        result = imagePath;
                        _logger.LogFileWritingSuccessful(imagePath);
                    }
                    catch
                    {
                        _logger.LogFileWritingFailed(imagePath);
                    }
                }
            }
            return result;
        }

        public async Task<string> SavePostImageAsync(long postId, IFormFile formFileForImage)
        {
            string result = "";
            if (formFileForImage != null)
            {
                string imageType = formFileForImage.FileName.Substring(formFileForImage.FileName.LastIndexOf('.') + 1).ToLower();
                if (AllowedImageTypes.Contains(imageType))
                {
                    string imagePath = Path.Combine(PostsImagesDirectory, $"{postId}.{imageType}");
                    try
                    {
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await formFileForImage.CopyToAsync(fileStream);
                        }
                        result = imagePath;
                        _logger.LogFileWritingSuccessful(imagePath);
                    }
                    catch
                    {
                        _logger.LogFileWritingFailed(imagePath);
                    }
                }
            }
            return result;
        }

        public bool DeleteFileAsync(string filePath)
        {
            bool deleted = false;
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    deleted = true;
                    _logger.LogFileDeletingSuccessful(filePath);
                }
                catch
                {
                    _logger.LogFileDeletingFailed(filePath);
                }
            }
            else
            {
                deleted = true;
            }
            return deleted;
        }

        public string GetImageInBase64(string imagePath)
        {
            string result = "";
            if (File.Exists(imagePath))
            {
                byte[] bytes = Array.Empty<byte>();
                FileStream fileStream = File.Open(imagePath, FileMode.Open);
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    bytes = binaryReader.ReadBytes((int)fileStream.Length); // Выдержит файл до 2 ГБ
                }
                fileStream.Close();
                result = Convert.ToBase64String(bytes);
            }
            return result;
        }
    }
}
