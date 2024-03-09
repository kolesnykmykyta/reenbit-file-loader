using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    [StorageAccount("BlobConnectionString")]
    public class BlobStorageMonitor
    {
        [FunctionName("BlobStorageMonitor")]
        public void Run([BlobTrigger("files-storage/{name}", Connection = "BlobStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
