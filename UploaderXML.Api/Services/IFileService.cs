using UploaderXML.Api.DTOs;

namespace UploaderXML.Api.Services
{
    public interface IFileService
    {
        public Task<FileUploadResponseDto> PostFileAsync(IFormFile fileData, string savePath, bool overwriteExistingFile);
    }
}
