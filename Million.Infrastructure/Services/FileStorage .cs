using Million.Application.Interfaces;

namespace Million.Infrastructure.Services
{
    public class FileStorage : IFileStorage
    {
        public async Task<string> SaveFileAsync(string fileBase64, string fileName, CancellationToken ct = default)
        {
            byte[] fileBytes = Convert.FromBase64String(fileBase64);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            await File.WriteAllBytesAsync(filePath, fileBytes, ct);

            return filePath;
        }
    }
}
