using MediatR;
using Microsoft.AspNetCore.Http;

namespace CloudCanvas.Application.Posts.Photos.Commands.UploadFile
{
    public class UploadFileCommand(IFormFile file, string userId): IRequest<UploadFileResult>
    {
        public string UserId { get; set; } = userId;
        public IFormFile File { get; set; } = file;
        public Stream Stream => File.OpenReadStream();
    }
}