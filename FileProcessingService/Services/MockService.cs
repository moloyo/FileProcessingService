using FileProcessingService.Interfaces;

namespace FileProcessingService.Services
{
    public class MockService : ITranscriptorService
    {
        public virtual async Task<string> GetTranscriptAsync()
        {
            await Task.Delay(1000);

            if (Random.Shared.Next(20) == 0)
            {
                throw new Exception();
            }

            return Random.Shared.GetItems(MockTests, 1).Single();
        }

        private string[] MockTests = [
            "Patient number 1 has a case of Gaucher disease",
            "Patient number 2 has a case of Becker muscular dystrophy",
            "Patient number 3 has a case of Angelman syndrome",
            "Patient number 4 has a case of Phenylketonuria"
        ];
    }
}
