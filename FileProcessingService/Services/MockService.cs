using FileProcessingService.Interfaces;

namespace FileProcessingService.Services
{
    public class MockService : ITranscriptorService
    {
        public virtual async Task<string> GetTranscriptAsync()
        {
            await Task.Delay(1000);

            var rand = new Random();

            if (rand.Next(20) == 0)
            {
                throw new Exception();
            }

            return MockTests[rand.Next(4) / 1];
        }

        private string[] MockTests = [
            "Patient number 1 has a case of Gaucher disease",
            "Patient number 2 has a case of Becker muscular dystrophy",
            "Patient number 3 has a case of Angelman syndrome",
            "Patient number 4 has a case of Phenylketonuria"
        ];
    }
}
