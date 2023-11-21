namespace UploaderXML.Api.DTOs
{
    public class FileDetailsDto
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Stream FileStream { get; set; }

        public string JsonText { get; set; }

        public bool OverwriteExistingFile { get; set; }
    }
}
