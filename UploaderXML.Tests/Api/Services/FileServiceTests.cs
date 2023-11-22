using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Xml;
using UploaderXML.Api.BOs;
using UploaderXML.Api.Constants;
using UploaderXML.Api.Services;

namespace UploaderXML.Tests.Api.Services
{
    public class FileServiceTests
    {
        private const string validContent = "<note><to>Me</to><body>Yess!</body></note>";

        private readonly Mock<IFileWriter> fileWriterMock = new();

        [Fact]
        public async Task PostFileAsync_WhenFileDataIsMissing_ShouldReturnValidationError()
        {
            // Arrange
            var targetService = new FileService(fileWriterMock.Object);

            // Act
            var result = await targetService.PostFileAsync(null, null, false);

            // Assert
            result.HasValidationError.Should().BeTrue();
            result.ValidationError.Should().Be(ValidationErrors.NoFile);
        }

        [Fact]
        public async Task PostFileAsync_WhenNotValidXml_ShouldReturnValidationError()
        {
            // Arrange
            var content = "Fake content";
            var fileData = MockFileData(content);

            var targetService = new FileService(fileWriterMock.Object);

            // Act
            var result = await targetService.PostFileAsync(fileData, null, false);

            // Assert
            result.HasValidationError.Should().BeTrue();
            result.ValidationError.Should().Be(ValidationErrors.InvalidXml);
        }

        [Fact]
        public async Task PostFileAsync_WhenFileExistsAndDontOverwriteExistingFileProvided_ShouldReturnValidationError()
        {
            // Arrange
            var fileData = MockFileData(validContent);

            fileWriterMock
                .Setup(x => x.FileExists(It.IsAny<string>()))
                .Returns(true);
            var targetService = new FileService(fileWriterMock.Object);

            // Act
            var result = await targetService.PostFileAsync(fileData, null, false);

            // Assert
            result.HasValidationError.Should().BeTrue();
            result.ValidationError.Should().Be(ValidationErrors.ExistingFile);
        }

        [Fact]
        public async Task PostFileAsync_WhenSomeDiskOperationFails_ShouldReturnExceptionError()
        {
            // Arrange
            var fileData = MockFileData(validContent);
            var errorMessage = "Some error!";

            fileWriterMock
                .Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ThrowsAsync(new Exception(errorMessage));
            var targetService = new FileService(fileWriterMock.Object);

            // Act
            var result = await targetService.PostFileAsync(fileData, null, false);

            // Assert
            result.HasExceptions.Should().BeTrue();
            result.ExceptionMessage.Should().Be(errorMessage);
        }

        [Fact]
        public async Task PostFileAsync_WhenInvalidDirectoryProvided_ShouldReturnExceptionError()
        {
            // Arrange
            var fileData = MockFileData(validContent);
            var errorMessage = "Some error!";
            var invalidDirectoryPath = "Invalid path";

            fileWriterMock
                .Setup(x => x.CreateDirectory(invalidDirectoryPath))
                .Throws(new Exception(errorMessage));
            var targetService = new FileService(fileWriterMock.Object);

            // Act
            var result = await targetService.PostFileAsync(fileData, invalidDirectoryPath, false);

            // Assert
            result.HasExceptions.Should().BeTrue();
            result.ExceptionMessage.Should().Be(errorMessage);
        }

        [Fact]
        public async Task PostFileAsync_WhenValidDataProvided_ShouldReturnJsonResultWithoutErrors()
        {
            // Arrange
            var fileData = MockFileData(validContent);
            var document = new XmlDocument();
            document.LoadXml(validContent);
            var expectedResult = JsonConvert.SerializeXmlNode(document, Newtonsoft.Json.Formatting.Indented);
            var targetService = new FileService(fileWriterMock.Object);

            // Act
            var result = await targetService.PostFileAsync(fileData, null, true);

            // Assert
            result.HasExceptions.Should().BeFalse();
            result.HasValidationError.Should().BeFalse();
            result.JsonText.Should().Be(expectedResult);
        }

        private IFormFile MockFileData(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "test.ext");

            return file;
        }
    }
}
