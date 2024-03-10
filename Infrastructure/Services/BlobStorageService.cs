using Azure;
using Azure.Storage.Blobs.Models;
using Infrastructure.BlobAccess;
using Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IBlobStorage _blobStorage;

        public BlobStorageService(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task UploadFileToStorageAsync(Stream? file, string? originalName, string? email)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (string.IsNullOrWhiteSpace(originalName))
            {
                throw new ArgumentException("Original name is null, empty or whitespace", nameof(originalName));
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is null, empty or whitespace", nameof(email));
            }
            if (!Path.GetExtension(originalName).ToLower().Equals(".docx"))
            {
                throw new ArgumentException("File extension is not .docx", nameof(file));
            }

            string newName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}_{originalName}"
                .Replace(" ", "_");
            Dictionary<string, string> metadata = new Dictionary<string, string>
            {
                {"email", email },
            };

            await _blobStorage.UploadFileAsync(file, newName, metadata);
        }

        public string GetFileUrl(string? fileName, string? accessKey)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is null, empty or whitespace", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("Access key is null, empty or whitespace", nameof(fileName));
            }

            string sasToken;
            try
            {
                sasToken = _blobStorage.GenerateSasToken(fileName, accessKey, 3600);
            }
            catch (RequestFailedException)
            {
                throw new InvalidOperationException("Provided access key is wrong");
            }

            string storageName = _blobStorage.StorageName;
            string containerName = _blobStorage.ContainerName;

            string fileUrl = $"https://{storageName}.blob.core.windows.net/{containerName}/{fileName}?{sasToken}";
            return fileUrl;
        }

        public string GetBlobMetadata(string? fileName, string? metadataName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is null, empty or whitespace", nameof(fileName));
            }
            if (string.IsNullOrWhiteSpace(metadataName))
            {
                throw new ArgumentException("Metadata name is null, empty or whitespace", nameof(metadataName));
            }

            IDictionary<string, string> metadata = _blobStorage.GetBlobMetadata(fileName).Metadata;
            if (metadata == null)
            {
                throw new ArgumentException("File with specified name wasn't found", nameof(fileName));
            }
            if (metadata[metadataName] == null)
            {
                throw new InvalidOperationException("File with specified name exists, but needed metadata wasn't found");
            }

            return metadata[metadataName];
        }
    }
}
