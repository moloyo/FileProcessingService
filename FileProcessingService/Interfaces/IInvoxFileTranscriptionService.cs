namespace FileProcessingService.Interfaces
{
    public interface IInvoxFileTranscriptionService
    {
        Task<string> TranscriptFileAsync(FileStream file, string userName);
    }
}