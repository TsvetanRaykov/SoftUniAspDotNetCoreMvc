using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Vxp.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImage(IFormFile file, string fileName);
    }
}