namespace FileProcessingService.Interfaces
{
    public interface ITranscriptorService
    {
        Task<string> GetTranscriptAsync();
    }
}