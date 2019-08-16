namespace Vxp.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;

    public class CloudinaryService : ICoudinaryService
    {
        private readonly Cloudinary _cloudinaryUtility;

        public CloudinaryService(Cloudinary cloudinaryUtility)
        {
            this._cloudinaryUtility = cloudinaryUtility;
        }

        public async Task<string> UploadImage(IFormFile file, string fileName)
        {
            byte[] destinationData;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                destinationData = ms.ToArray();
            }

            UploadResult uploadResult = null;

            using (var ms = new MemoryStream(destinationData))
            {
                var imageUploadParams = new ImageUploadParams
                {
                    Folder = "product_images",
                    File = new FileDescription(fileName, ms)
                };

                uploadResult = this._cloudinaryUtility.Upload(imageUploadParams);
            }

            return uploadResult?.SecureUri.AbsoluteUri;
        }
    }
}