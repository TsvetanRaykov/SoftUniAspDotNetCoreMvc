namespace Vxp.Services.Data.Projects
{
    using Vxp.Data.Common.Enums;
    using Models;
    using System.Threading.Tasks;

    public interface IFilesService
    {
        Task<FileStoreDto> StoreFileToDatabaseAsync(FileStoreDto fileDto);
        bool ValidateFileType(string mimeType);
        Task<FileStoreDto> GetDocumentByIdAsync(int fileId);
        string GetNewApplicationUserFolder(string userId, DocumentType documentType);
        Task<bool> DeleteFileFromDataBaseAsync(int fileId);
    }
}