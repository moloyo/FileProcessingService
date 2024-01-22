using FileProcessingService.Interfaces;

namespace FileProcessingService.Services
{
    public class WindowsBackgroundService(IFileService fileService, ILogger<WindowsBackgroundService> logger) : BackgroundService
    {
        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <remarks>
        /// See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeUntilMidday(), stoppingToken);

                    fileService.ProcessFile();
                    logger.LogWarning("Processing file");
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);

                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Times the until midday.
        /// </summary>
        /// <returns></returns>
        private TimeSpan TimeUntilMidday()
        {
            return DateTime.Today.AddDays(1) - DateTime.Now;
        }
    }
}
