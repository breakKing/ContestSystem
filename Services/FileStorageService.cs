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
        private readonly string _storageDirectory = @"\Storage";
        private readonly string _imagesDirectory = @"\Images";
        private readonly string _contestsDirectory = @"\Contests";
        private readonly string _postsDirectory = @"\Posts";
        private readonly ILogger<FileStorageService> _logger;

        private string StorageDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _storageDirectory);
        private string ImagesDirectory => Path.Combine(StorageDirectory, _imagesDirectory);
        private string ContestsImagesDirectory => Path.Combine(ImagesDirectory, _contestsDirectory);
        private string PostsImagesDirectory => Path.Combine(ImagesDirectory, _postsDirectory);

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

        public async Task<bool> SaveContestImageAsync(long contestId, IFormFile formFileForImage)
        {
            bool status = false;
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
                        status = true;
                    }
                    catch
                    {
                        _logger.LogFileWritingFailed(imagePath);
                    }
                }
            }
            return status;
        }

        public async Task<bool> SavePostImageAsync(long postId, IFormFile formFileForImage)
        {
            bool status = false;
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
                        status = true;
                    }
                    catch
                    {
                        _logger.LogFileWritingFailed(imagePath);
                    }
                }
            }
            return status;
        }

        public string GetContestImageInBase64(long contestId)
        {
            string result = "";
            string[] images = Directory.GetFiles(ContestsImagesDirectory, $"{contestId}.*");
            if (images.Length > 0)
            {
                string imagePath = images[0];
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

        public string GetPostImageInBase64(long postId)
        {
            string result = "";
            string[] images = Directory.GetFiles(PostsImagesDirectory, $"{postId}.*");
            if (images.Length > 0)
            {
                string imagePath = images[0];
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
