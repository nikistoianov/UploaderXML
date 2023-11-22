using Newtonsoft.Json;
using System.Text;
using System.Xml;
using UploaderXML.Api.BOs;
using UploaderXML.Api.Constants;
using UploaderXML.Api.DTOs;

namespace UploaderXML.Api.Services
{
    public class FileService : IFileService
    {
        private readonly IFileWriter fileWriter;

        public FileService(IFileWriter fileWriter)
        {
            this.fileWriter = fileWriter;
        }

        public async Task<FileUploadResponseDto> PostFileAsync(IFormFile fileData, string savePath, bool overwriteExistingFile)
        {
            var result = new FileUploadResponseDto();
            try
            {
                if (fileData == null)
                {
                    result.ValidationError = ValidationErrors.NoFile;
                    return result;
                };

                var fileDetails = new FileDetailsDto()
                {
                    FileName = $"{fileData.FileName}.json",
                    FilePath = savePath ?? "",
                    FileStream = fileData.OpenReadStream(),
                    OverwriteExistingFile = overwriteExistingFile,
                };

                if (!IsValidXml(fileDetails.FileStream, out XmlDocument document))
                {
                    result.ValidationError = ValidationErrors.InvalidXml;
                    return result;
                }
                else if (!ValidateFileDetails(fileDetails))
                {
                    result.ValidationError = ValidationErrors.ExistingFile;
                    return result;
                }

                fileDetails.JsonText = JsonConvert.SerializeXmlNode(document, Newtonsoft.Json.Formatting.Indented);

                await CreateFile(fileDetails);
                result.JsonText = fileDetails.JsonText;
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = ex.Message;
            }

            return result;
        }

        private async Task CreateFile(FileDetailsDto fileDetails)
        {
            var fullPath = Path.Combine(fileDetails.FilePath, fileDetails.FileName);

            if (fileWriter.FileExists(fullPath))
            {
                fileWriter.FileDelete(fullPath);
            }

            if (!string.IsNullOrEmpty(fileDetails.FilePath) && !fileWriter.DirectoryExists(fileDetails.FilePath))
            {
                fileWriter.CreateDirectory(fileDetails.FilePath);
            }

            var content = new UTF8Encoding(true).GetBytes(fileDetails.JsonText);
            await fileWriter.WriteAsync(fullPath, content);
        }

        private bool IsValidXml(Stream fileStream, out XmlDocument document)
        {
            document = new XmlDocument();

            try
            {
                document.Load(fileStream);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ValidateFileDetails(FileDetailsDto fileDetails)
        {
            var fullPath = Path.Combine(fileDetails.FilePath, fileDetails.FileName);
            if (!fileDetails.OverwriteExistingFile && fileWriter.FileExists(fullPath))
            {
                return false;
            }

            return true;
        }
    }
}
