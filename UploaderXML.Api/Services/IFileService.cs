using UploaderXML.Api.Models;

namespace UploaderXML.Api.Services
{
    public interface IFileService
    {
        public Task<FileUploadResponseModel> PostFileAsync(IFormFile fileData, string savePath, bool overwriteExistingFile);
    }
}
