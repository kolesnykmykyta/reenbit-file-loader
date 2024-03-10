using System.IO;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public class BlobStorageMonitor
    {
        private readonly ILogger<BlobStorageMonitor> _logger;
        private readonly IEmailService _emailService;
        private readonly IBlobStorageService _storageService;

        public BlobStorageMonitor(ILogger<BlobStorageMonitor> logger, IEmailService emailService, IBlobStorageService storageService)
        {
            _logger = logger;
            _emailService = emailService;
            _storageService = storageService;
        }

        [Function(nameof(BlobStorageMonitor))]
        public async Task Run([BlobTrigger("files-storage/{name}", Connection = "BlobConnectionString")] Stream stream, string name)
        {
            try
            {
                _logger.LogInformation($"{name} blob processing started.");
                string accessKey = Environment.GetEnvironmentVariable("BlobAccessKey")!;

                string fileUrl = _storageService.GetFileUrl(name, accessKey);
                string receiver = _storageService.GetBlobMetadata(name, "email");

                string message = "<html><body>" +
                    "<h2>Link to your file</h2>" +
                    $"<p>Your file was successully uploaded to the BLOB storage. To get access to your file, use the next link: <br>{fileUrl}." +
                    "<br>This link is available for one hour.</p>" +
                    "</body></html>";

                _emailService.SendEmail("Link to your file", message, receiver);

                _logger.LogInformation($"Successfully send the email for the {name} blob.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while processing {name}: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation($"{name} blob processing ended.");
            }
        }
    }
}
