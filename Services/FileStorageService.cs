using ContestSystem.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
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
        private readonly string _chatsDirectory = @"Chats";
        private readonly ILogger<FileStorageService> _logger;

        private string ContentRootPath { get; set; }
        private string StorageDirectory { get; set; }
        private string ImagesDirectory { get; set; }
        private string ContestsImagesDirectory { get; set; }
        private string PostsImagesDirectory { get; set; }
        private string ChatsImagesDirectory { get; set; }

        private List<string> AllowedImageTypes => new List<string>
        {
            "jpg",
            "jpeg",
            "png",
            "bmp",
            "tiff"
        };

        public FileStorageService(ILogger<FileStorageService> logger, IHostEnvironment environment)
        {
            _logger = logger;

            ContentRootPath = environment.ContentRootPath;
            StorageDirectory = GeneratePath(ContentRootPath, _storageDirectory);
            ImagesDirectory = GeneratePath(StorageDirectory, _imagesDirectory);
            ContestsImagesDirectory = GeneratePath(ImagesDirectory, _contestsDirectory);
            PostsImagesDirectory = GeneratePath(ImagesDirectory, _postsDirectory);
            ChatsImagesDirectory = GeneratePath(ImagesDirectory, _chatsDirectory);

            EnsureDirectoryCreated(StorageDirectory);
            EnsureDirectoryCreated(ImagesDirectory);
            EnsureDirectoryCreated(ContestsImagesDirectory);
            EnsureDirectoryCreated(PostsImagesDirectory);
            EnsureDirectoryCreated(ChatsImagesDirectory);
        }

        private string GeneratePath(params string[] pathPieces)
        {
            return Path.Combine(pathPieces);
        }

        private string GetAbsolutePath(string path)
        {
            return Path.Combine(ContentRootPath, path);
        }

        private string GetRelativePath(string path)
        {
            return Path.GetRelativePath(ContentRootPath, path);
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
            return string.IsNullOrWhiteSpace(result) ? result : GetRelativePath(result);
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
            return string.IsNullOrWhiteSpace(result) ? result : GetRelativePath(result);
        }

        public async Task<string> SaveChatImageAsync(long chatId, IFormFile formFileForImage)
        {
            string result = "";
            if (formFileForImage != null)
            {
                string imageType = formFileForImage.FileName.Substring(formFileForImage.FileName.LastIndexOf('.') + 1).ToLower();
                if (AllowedImageTypes.Contains(imageType))
                {
                    string imagePath = Path.Combine(ChatsImagesDirectory, $"{chatId}.{imageType}");
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
            return string.IsNullOrWhiteSpace(result) ? result : GetRelativePath(result);
        }

        public bool DeleteFileAsync(string filePath)
        {
            bool deleted = false;
            filePath = GetAbsolutePath(filePath);
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
            imagePath = GetAbsolutePath(imagePath);
            if (File.Exists(imagePath))
            {
                try
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
                catch { }
            }
            return result;
        }
    }
}
