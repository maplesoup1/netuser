using Google.Cloud.Storage.V1;
using Titube.Interfaces;

namespace Titube.Services
{
    public class GoogleCloudStorageService : IStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public GoogleCloudStorageService(IConfiguration configuration)
        {
            _bucketName = configuration["GoogleCloud:BucketName"] 
                ?? throw new ArgumentNullException("GoogleCloud:BucketName is required in appsettings.json");
            
            _storageClient = StorageClient.Create();
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("can not be null or empty", nameof(file));

            try
            {
                string fileName = $"{folder}/{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}".TrimStart('/');
                
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    
                    await _storageClient.UploadObjectAsync(
                        _bucketName,
                        fileName,
                        file.ContentType,
                        memoryStream);
                    
                    return fileName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"fail: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GetFileAsync(string objectName)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await _storageClient.DownloadObjectAsync(
                        _bucketName,
                        objectName,
                        memoryStream);
                    
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"fail: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string objectName)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"delete fail: {ex.Message}");
                return false;
            }
        }
    }
}