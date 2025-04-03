namespace Titube.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
        
        Task<byte[]> GetFileAsync(string objectName);
        
        Task<bool> DeleteFileAsync(string objectName);
    }
}