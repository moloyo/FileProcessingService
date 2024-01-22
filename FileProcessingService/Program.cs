using FileProcessingService.Interfaces;
using FileProcessingService.Services;

namespace FileProcessingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = ".NET File Service";
            });

            builder.Services.AddSingleton<ITranscriptorService, MockService>();
            builder.Services.AddSingleton<IFileService, FileService>();
            builder.Services.AddSingleton<IInvoxFileTranscriptionService, InvoxFileTranscriptionService>();
            builder.Services.AddHostedService<WindowsBackgroundService>();

            var host = builder.Build();
            host.Run();
        }
    }
}