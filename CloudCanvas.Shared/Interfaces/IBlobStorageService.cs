using CloudCanvas.Constants;

namespace CloudCanvas.Interfaces;

public interface IBlobStorageService
{
    public Task UploadAsync(string containerName, Stream fileStream, string filename);
    public Task<List<string>> GetUrlsAsync(string containerName);
}
