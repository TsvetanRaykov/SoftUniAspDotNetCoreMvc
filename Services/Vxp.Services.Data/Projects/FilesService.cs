﻿namespace Vxp.Services.Data.Projects
{
    using Vxp.Data.Common.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Models;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Document = Vxp.Data.Models.Document;

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

        public async Task<FileStoreDto> StoreFileToDatabaseAsync(FileStoreDto fileDto)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(fileDto.OriginalFileName);

            var directoryPath = this.GetNewApplicationUserFolder(fileDto.UserId, fileDto.Type);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            fileDto.Location = Path.Combine(directoryPath, fileName);

            var newDocument = AutoMapper.Mapper.Map<Document>(fileDto);
            newDocument.DocumentDate = DateTime.UtcNow;

            await this._documentsRepository.AddAsync(newDocument);
            await this._documentsRepository.SaveChangesAsync();

            return AutoMapper.Mapper.Map<FileStoreDto>(newDocument);
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

        public string GetNewApplicationUserFolder(string userId, DocumentType documentType)
        {
            var configPath = this._configuration["FileStorage:AppUsers"];
            var directoryPath = Path.GetFullPath(configPath);

            directoryPath = Path.Combine(
                directoryPath,
                userId,
                documentType.ToString());

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }

        public async Task<bool> DeleteFileFromDataBaseAsync(int fileId)
        {
            var fileFromDb = await this._documentsRepository.GetByIdWithDeletedAsync(fileId);
            if (fileFromDb == null)
            {
                return false;
            }

            this._documentsRepository.Delete(fileFromDb);
            await this._documentsRepository.SaveChangesAsync();
            return true;
        }
    }
}