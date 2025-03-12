using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Wallet.Services.Abstract;

namespace Wallet.Services.Concrete;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["FileStorage:AttachmentPath"] 
            ?? throw new ArgumentNullException("FileStorage:AttachmentPath configuration is missing");
    }

    public async Task<(string fileName, string filePath)> SaveFileAsync(IFormFile file, string folder)
    {
        // Güvenli dosya adı oluştur
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        
        // Klasör yolunu oluştur
        var folderPath = Path.Combine(_basePath, folder);
        var filePath = Path.Combine(folderPath, fileName);

        // Klasörü oluştur (yoksa)
        Directory.CreateDirectory(folderPath);

        // Dosyayı kaydet
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return (fileName, filePath);
    }

    public async Task<byte[]> GetFileAsync(string fileName, string folder)
    {
        var filePath = Path.Combine(_basePath, folder, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {fileName}");

        return await File.ReadAllBytesAsync(filePath);
    }

    public async Task DeleteFileAsync(string fileName, string folder)
    {
        var filePath = Path.Combine(_basePath, folder, fileName);

        if (File.Exists(filePath))
            await Task.Run(() => File.Delete(filePath));
    }

    public bool FileExists(string fileName, string folder)
    {
        var filePath = Path.Combine(_basePath, folder, fileName);
        return File.Exists(filePath);
    }

    private string SanitizeFileName(string fileName)
    {
        // Güvenli olmayan karakterleri temizle
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = string.Join("_", fileName.Split(invalidChars));
        
        // Maksimum uzunluğu sınırla
        if (safeName.Length > 100)
            safeName = safeName.Substring(0, 100);
            
        return safeName;
    }
}