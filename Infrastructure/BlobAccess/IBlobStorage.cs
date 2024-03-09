using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BlobAccess
{
    public interface IBlobStorage
    {
        public Task UploadFileAsync(Stream file, string fileName, Dictionary<string, string>? metadata = null);

        public string GenerateSasToken(string? fileName, string? accessKey, int expireTime);

        public BlobProperties GetBlobMetadata(string? fileName);

        public string StorageName { get; }

        public string ContainerName { get; }
    }
}
