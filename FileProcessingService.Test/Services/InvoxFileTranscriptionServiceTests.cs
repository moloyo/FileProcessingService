using FileProcessingService.Interfaces;
using FileProcessingService.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FileProcessingService.Tests.Services
{
    public class InvoxFileTranscriptionServiceTests : IDisposable
    {
        private readonly Mock<ITranscriptorService> _transcriptorService;
        private readonly IConfiguration _configuration;
        private readonly InvoxFileTranscriptionService _invoxFileTranscriptionService;

        public InvoxFileTranscriptionServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"MaxNumberOfRetries", "3"},
                {"FileDirectory", "./InvoxFileTranscriptionService_directory"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _transcriptorService = new Mock<ITranscriptorService>();

            Directory.CreateDirectory(_configuration["FileDirectory"]);

            _invoxFileTranscriptionService = new InvoxFileTranscriptionService(_transcriptorService.Object, _configuration);
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
        public async void TranscriptFileAsync_WithValidMp3Files_CallsTranscriptFileAsyncFromMockService()
        {
            // Arrange
            _transcriptorService.Setup(m => m.GetTranscriptAsync()).ReturnsAsync("Mocked text");
            using var stream = new FileStream(Path.Combine(_configuration["FileDirectory"], "test.mp4"), FileMode.Create);
            var username = "user";

            // Act
            var result = await _invoxFileTranscriptionService.TranscriptFileAsync(stream, username);

            // Assert
            _transcriptorService.Verify(m => m.GetTranscriptAsync(), Times.Once);
        }

        [Fact] 
        public async void TranscriptFileAsync_WithInvalidMp3Files_CallsTranscriptFileAsyncFromMockServiceThreeTimesIfError()
        {
            // Arrange
            _transcriptorService.Setup(m => m.GetTranscriptAsync()).ThrowsAsync(new Exception());
            using var stream = new FileStream(Path.Combine(_configuration["FileDirectory"], "test2.mp4"), FileMode.Create);
            var username = "user";

            // Act
            try
            {
                var result = await _invoxFileTranscriptionService.TranscriptFileAsync(stream, username);
            }
            catch
            {
                
            }

            // Assert
            _transcriptorService.Verify(m => m.GetTranscriptAsync(), Times.Exactly(3));
        }
    }
}
