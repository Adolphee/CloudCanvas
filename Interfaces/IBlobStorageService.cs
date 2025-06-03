using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CloudCanvas.Interfaces;

public interface IBlobStorageService
{
    public Task UploadAsync(IFormFile formFile);
    public Task<List<string>> GetUrlsAsync();
}
