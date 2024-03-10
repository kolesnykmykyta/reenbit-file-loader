using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BlobAccess
{
    /// <summary>
    /// Provides access to the specified container in the Azure storage.
    /// </summary>
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly BlobContainerClient _containerClient;

        public BlobStorage(string connectionString, string containerName)
        {
            _serviceClient = new BlobServiceClient(connectionString);
            _containerClient = _serviceClient.GetBlobContainerClient(containerName);
        }

        /// <summary>
        /// Returns the name of the Azure storage.
        /// </summary>
        public string StorageName
        {
            get
            {
                return _serviceClient.AccountName;
            }
        }

        /// <summary>
        /// Returns the name of the Azure container.
        /// </summary>
        public string ContainerName
        {
            get
            {
                return _containerClient.Name;
            }
        }

        /// <summary>
        /// Uploads provided file to the Azure storage.
        /// </summary>
        /// <param name="file">File as a Stream object.</param>
        /// <param name="fileName">Name of the file which is used in the storage.</param>
        /// <param name="metadata">Metadata of the file as a Dictionary object.</param>
        public async Task UploadFileAsync(Stream file, string fileName, Dictionary<string, string>? metadata = null)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            BlobUploadOptions options = new BlobUploadOptions
            {
                Metadata = metadata,
            };

            await blobClient.UploadAsync(file, options);
        }

        /// <summary>
        /// Generates SAS token for the file.
        /// </summary>
        /// <param name="fileName">Name of the needed file.</param>
        /// <param name="accessKey">Key for the access to the Azure storage.</param>
        /// <param name="expireTime">Expire time of SAS the token (in seconds).</param>
        public string GenerateSasToken(string? fileName, string? accessKey, int expireTime)
        {
            string storageName = _serviceClient.AccountName;
            string containerName = _containerClient.Name;

            BlobSasBuilder builder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                ExpiresOn = DateTime.Now.AddSeconds(expireTime),
            };
            builder.SetPermissions(BlobSasPermissions.Read);

            StorageSharedKeyCredential credential = new(storageName, accessKey);
            string sasToken = builder.ToSasQueryParameters(credential).ToString();

            return sasToken;
        }

        /// <summary>
        /// Provides the metadata of the file.
        /// </summary>
        /// <param name="fileName">Name of the needed file.</param>
        /// <returns>Metadata of the file as a Dictionary object.</returns>
        public IDictionary<string, string> GetBlobMetadata(string? fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            BlobProperties properties = blobClient.GetProperties();

            return properties.Metadata;
        }
    }
}
