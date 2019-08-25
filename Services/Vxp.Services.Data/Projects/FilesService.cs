namespace Vxp.Services.Data.Projects
{
    using Microsoft.Extensions.Configuration;
    using Models;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Document = Vxp.Data.Models.Document;
    using Microsoft.EntityFrameworkCore;

    public class FilesService : IFilesService
    {
        private readonly IConfiguration _configuration;
        private readonly IDeletableEntityRepository<Document> _documentsRepository;
        public FilesService(
            IConfiguration configuration,
            IDeletableEntityRepository<Document> documentsRepository)
        {
            this._configuration = configuration;
            this._documentsRepository = documentsRepository;
        }

        public async Task<string> StoreFileAsync(FileStoreDto fileDto)
        {
            var configPath = this._configuration["FileStorage:AppUsers"];
            var directoryPath = Path.GetFullPath(configPath);

            directoryPath = Path.Combine(
                directoryPath,
                fileDto.UserId,
                fileDto.Type.ToString());

            var fileName = Guid.NewGuid() + Path.GetExtension(fileDto.FormFile.FileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            fileDto.Location = Path.Combine(directoryPath, fileName);

            using (var fileStream = new FileStream(fileDto.Location, FileMode.Create, FileAccess.Write))
            {
                fileDto.FormFile.CopyTo(fileStream);
            }

            var newDocument = AutoMapper.Mapper.Map<Document>(fileDto);
            newDocument.DocumentDate = DateTime.UtcNow;

            await this._documentsRepository.AddAsync(newDocument);
            await this._documentsRepository.SaveChangesAsync();

            return fileDto.Location;
        }

        public bool ValidateFileType(string mimeType)
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly?.GetManifestResourceStream("Vxp.Web.Resources.AllowedMimeTypes.csv");

            if (resourceStream == null)
            {
                return false;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var allowedType = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    if (allowedType == mimeType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<FileStoreDto> GetDocumentByIdAsync(int fileId)
        {
            var fileFromDb = await this._documentsRepository
                .AllAsNoTrackingWithDeleted()
                .Include(f => f.Project)
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (fileFromDb == null)
            {
                return default;
            }

            return AutoMapper.Mapper.Map<FileStoreDto>(fileFromDb);
        }
    }
}