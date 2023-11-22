using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using UploaderXML.Web.Models;

namespace UploaderXML.Web.Pages.Components
{
    public partial class FileUploader
    {
        RadzenUpload upload;
        private int progress;
        private string apiResponse = "";

        [Inject]
        public ApiSettings ApiSettings { get; set; }

        void OnProgress(UploadProgressArgs args)
        {
            progress = args.Progress;
        }

        void OnChange(UploadChangeEventArgs args)
        {
            progress = 0;
            apiResponse = "";
        }

        void OnComplete(UploadCompleteEventArgs args)
        {
            apiResponse = args.RawResponse;
        }

        void UploadError(UploadErrorEventArgs args)
        {
            apiResponse = args.Message;
        }

        string GetUrl()
        {
            return $"{ApiSettings.ApiAddress}?filePath={ApiSettings.SavePath}&overwriteExistingFile={ApiSettings.OverwriteExistingFile}";
        }
    }
}
