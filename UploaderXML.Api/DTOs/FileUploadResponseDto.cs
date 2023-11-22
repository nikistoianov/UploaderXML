namespace UploaderXML.Api.DTOs
{
    public class FileUploadResponseDto
    {
        public string ExceptionMessage { get; set; }

        public string JsonText { get; set; }

        public string ValidationError { get; set; }

        public bool HasValidationError => !string.IsNullOrEmpty(ValidationError);

        public bool HasExceptions => !string.IsNullOrEmpty(ExceptionMessage);
    }
}
