using System.ComponentModel.DataAnnotations;

namespace UploaderXML.Api.Models
{
    public class FileUploadModel
    {
        [Required]
        public IFormFile FileData { get; set; }

        public string SavePath { get; set; }

        public bool OverwriteExistingFile { get; set; }
    }
}
