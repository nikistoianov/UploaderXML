namespace UploaderXML.Api.Models
{
    public class FileUploadResponseModel
    {
        public string ExceptionMessage { get; set; }

        public string ValidationError { get; set; }

        public bool HasValidationError => !string.IsNullOrEmpty(ValidationError);

        public bool HasExceptions => !string.IsNullOrEmpty(ExceptionMessage);
    }
}
