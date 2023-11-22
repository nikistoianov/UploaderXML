using Microsoft.AspNetCore.Mvc;
using UploaderXML.Api.Services;

namespace UploaderXML.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService fileService;

        public FilesController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        /// <summary>
        /// Upload XML file and convert to JSON
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> ConvertXmlToJson(IFormFile file, string filePath, bool overwriteExistingFile)
        {
            var response = await fileService.PostFileAsync(file, filePath, overwriteExistingFile);
            if(response.HasValidationError)
            {
                return BadRequest(response.ValidationError);
            }
            else if(response.HasExceptions)
            {
                return InternalServerError(response.ExceptionMessage);
            }

            return Ok(response.JsonText);
        }

        protected ActionResult InternalServerError(string message)
            => StatusCode(StatusCodes.Status500InternalServerError, message);
    }
}
