using FileProcessingService.Interfaces;
using FileProcessingService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FileProcessingService.Tests.Services
{
    public class FileServiceTests : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly Mock<IInvoxFileTranscriptionService> _invoxFileTranscriptionServiceMock;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            var loggerMock = new Mock<ILogger<FileService>>();

            var inMemorySettings = new Dictionary<string, string> {
                {"Validations:MinSizeBytes", "1"},
                {"Validations:MaxSizeBytes", "3145728"},
                {"FileDirectory", "./fake_directory"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var fileName = "testFile.mp3";
            var filePath = Path.Combine(_configuration["FileDirectory"], fileName);
            var fileContent = "Mocked file content";

            Directory.CreateDirectory(_configuration["FileDirectory"]);
            File.WriteAllText(filePath, fileContent);

            _invoxFileTranscriptionServiceMock = new Mock<IInvoxFileTranscriptionService>();

            _fileService = new FileService(_invoxFileTranscriptionServiceMock.Object, _configuration, loggerMock.Object);
        }

        public void Dispose()
        {
            var files = Directory.GetFiles(_configuration["FileDirectory"]);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            Directory.Delete(_configuration["FileDirectory"]);
        }

        [Fact]
        public void ProcessFile_WithValidMp3Files_CallsTranscriptFileAsyncForEachFile()
        {
            // Arrange
            _invoxFileTranscriptionServiceMock
                .Setup(i => i.TranscriptFileAsync(It.IsAny<FileStream>(), It.IsAny<string>()))
                .ReturnsAsync("Transcription");

            // Act
            _fileService.ProcessFile();

            // Assert
            _invoxFileTranscriptionServiceMock
                .Verify(i => i.TranscriptFileAsync(It.IsAny<FileStream>(), Environment.UserName), Times.Once);
        }

        [Fact]
        public void ProcessFile_WithValidMp3Files_CreatesTranscriptionForEachFile()
        {
            // Arrange
            _invoxFileTranscriptionServiceMock
                .Setup(i => i.TranscriptFileAsync(It.IsAny<FileStream>(), It.IsAny<string>()))
                .ReturnsAsync("Transcription");

            // Act
            _fileService.ProcessFile();

            // Assert
            Assert.True(File.Exists($"{_configuration["FileDirectory"]}/testFile.txt"));
        }

        [Fact]
        public void ProcessFile_WithInvalidMp3Files_CallsTranscriptFileAsyncForEachFile()
        {
            // Arrange
            var fileName = "invalidTestFile.mp4";
            var filePath = Path.Combine(_configuration["FileDirectory"], fileName);
            var fileContent = "Mocked invalid file content";

            File.WriteAllText(filePath, fileContent);

            // Act
            _fileService.ProcessFile();

            // Assert
            _invoxFileTranscriptionServiceMock
                .Verify(i => i.TranscriptFileAsync(It.IsAny<FileStream>(), Environment.UserName), Times.Never);
            Assert.True(!File.Exists("invalidTestFile.txt"));
        }
    }
}
