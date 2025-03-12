using Microsoft.AspNetCore.Http;

namespace Wallet.Services.Abstract
{
    public interface IFileStorageService
    {
        Task<(string fileName, string filePath)> SaveFileAsync(IFormFile file, string folder);
        Task<byte[]> GetFileAsync(string fileName, string folder);
        Task DeleteFileAsync(string fileName, string folder);
        bool FileExists(string fileName, string folder);
    }
} 