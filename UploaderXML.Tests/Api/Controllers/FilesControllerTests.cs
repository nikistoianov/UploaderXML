using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploaderXML.Api.Controllers;
using UploaderXML.Api.DTOs;
using UploaderXML.Api.Services;

namespace UploaderXML.Tests.Api.Controllers
{
    public class FilesControllerTests
    {
        private readonly Mock<IFileService> fileServiceMock = new();

        [Fact]
        public async Task ConvertXmlToJson_WhenHasValidationError_ShouldReturnBadRequest()
        {
            // Arrange
            var response = new FileUploadResponseDto()
            {
                ValidationError = "Some error!"
            };

            fileServiceMock
                .Setup(x => x.PostFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(response);
            var targetController = new FilesController(fileServiceMock.Object);

            // Act
            var result = await targetController.ConvertXmlToJson(null, null, false);
            
            // Assert
            result.As<ObjectResult>().StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.As<ObjectResult>().Value.Should().Be(response.ValidationError);
        }

        [Fact]
        public async Task ConvertXmlToJson_WhenHasExceptionError_ShouldReturnInternalServerError()
        {
            // Arrange
            var response = new FileUploadResponseDto()
            {
                ExceptionMessage = "Some error!"
            };

            fileServiceMock
                .Setup(x => x.PostFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(response);
            var targetController = new FilesController(fileServiceMock.Object);

            // Act
            var result = await targetController.ConvertXmlToJson(null, null, false);

            // Assert
            result.As<ObjectResult>().StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.As<ObjectResult>().Value.Should().Be(response.ExceptionMessage);
        }

        [Fact]
        public async Task ConvertXmlToJson_WhenHasNoErrors_ShouldReturnOkObjectResult()
        {
            // Arrange
            var response = new FileUploadResponseDto()
            {
                JsonText = "Json result"
            };

            fileServiceMock
                .Setup(x => x.PostFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(response);
            var targetController = new FilesController(fileServiceMock.Object);

            // Act
            var result = await targetController.ConvertXmlToJson(null, null, false);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<ObjectResult>().StatusCode.Should().Be(StatusCodes.Status200OK);
            result.As<ObjectResult>().Value.Should().Be(response.JsonText);
        }
    }
}
