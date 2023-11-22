using Newtonsoft.Json;
using System.Text;
using System.Xml;
using UploaderXML.Api.Constants;
using UploaderXML.Api.DTOs;

namespace UploaderXML.Api.Services
{
    public class FileService : IFileService
    {
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

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            if (!string.IsNullOrEmpty(fileDetails.FilePath) && !Directory.Exists(fileDetails.FilePath))
            {
                Directory.CreateDirectory(fileDetails.FilePath);
            }

            using var fileStream = File.Create(fullPath);
            var content = new UTF8Encoding(true).GetBytes(fileDetails.JsonText);
            await fileStream.WriteAsync(content);
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
            if (!fileDetails.OverwriteExistingFile && File.Exists(fullPath))
            {
                return false;
            }

            return true;
        }
    }
}
