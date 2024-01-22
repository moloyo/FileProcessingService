using FileProcessingService.Interfaces;

namespace FileProcessingService.Services
{
    /// <summary>
    /// This is a mock InvoxFileTranscriptionService. It's not actually sending the files, but is randomlt returning a different text and sometimes throwing an error
    /// </summary>
    public class InvoxFileTranscriptionService(ITranscriptorService transcriptorService, IConfiguration configuration) : IInvoxFileTranscriptionService
    {
        /// <summary>
        /// Transcripts the file asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Transcription of the file content</returns>
        /// <exception cref="System.Exception">Error after {numberOfRetries} tries</exception>
        public virtual async Task<string> TranscriptFileAsync(FileStream file, string userName)
        {
            var numberOfRetries = 0;

            while (numberOfRetries < configuration.GetValue<int>("MaxNumberOfRetries"))
            {
                try
                {
                    return await transcriptorService.GetTranscriptAsync();
                }
                catch (Exception)
                {
                    numberOfRetries++;
                }
            }

            throw new Exception($"Error after {numberOfRetries} tries");
        }
    }
}
