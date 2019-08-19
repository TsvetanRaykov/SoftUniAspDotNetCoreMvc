using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Vxp.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImage(IFormFile file, string fileName);
    }
}