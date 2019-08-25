namespace Vxp.Services.Data.Projects
{
    using System.Threading.Tasks;
    using Models;

    public interface IFilesService
    {
        Task<string> StoreFileAsync(FileStoreDto fileDto);
        bool ValidateFileType(string mimeType);

        Task<FileStoreDto> GetDocumentByIdAsync(int fileId);
    }
}