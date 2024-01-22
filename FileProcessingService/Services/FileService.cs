using FileProcessingService.Interfaces;

namespace FileProcessingService.Services
{
    public class FileService(IInvoxFileTranscriptionService invoxFileTranscriptionService, IConfiguration configuration, ILogger<FileService> logger) : IFileService
    {
        /// <summary>
        /// Processes the files in the configured directory.
        /// </summary>
        public void ProcessFile()
        {
            var files = Directory.GetFiles(configuration["FileDirectory"]!)
                .Where(fn => Path.GetExtension(fn) == ".mp3")
                .Where(fn => IsInFileSizeRange(new FileInfo(fn).Length));

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 3 }, async fn =>
            {
                using var file = File.OpenRead(fn);
                var userName = Environment.UserName;

                logger.LogInformation($"Using profile for {userName} at {DateTime.Now} for file {fn}");

                var transcript = await invoxFileTranscriptionService.TranscriptFileAsync(file, userName);

                SaveTranscriptFile(transcript, file.Name);
            });
        }

        /// <summary>
        /// Saves the transcript file.
        /// </summary>
        /// <param name="transcript">The transcript.</param>
        /// <param name="name">The original file name.</param>
        private static void SaveTranscriptFile(string transcript, string name)
        {
            File.WriteAllText(Path.ChangeExtension(name, "txt"), transcript);
        }

        /// <summary>
        /// Determines whether [is in file size range] [the specified file size].
        /// </summary>
        /// <param name="fileSize">Size of the file.</param>
        /// <returns>
        ///   <c>true</c> if [is in file size range] [the specified file size]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInFileSizeRange(long fileSize)
        {
            var minSize = configuration.GetValue<long>("Validations:MinSizeBytes");
            var maxSize = configuration.GetValue<long>("Validations:MaxSizeBytes");

            return minSize <= fileSize && fileSize <= maxSize;
        }
    }
}
