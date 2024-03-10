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
    /// <summary>
    /// Contains methods for working with Azure blob storage.
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IBlobStorage _blobStorage;

        public BlobStorageService(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        /// <summary>
        /// Uploads file to the Azure blob storage.
        /// </summary>
        /// <param name="file">File as a Stream object.</param>
        /// <param name="originalName">Original name of the file.</param>
        /// <param name="email">Email that will be stored as a metadata value.</param>
        /// <exception cref="ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="ArgumentException">Thrown when method arguments either null or invalid.</exception>
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

        /// <summary>
        /// Generates one-hour URL to the file with the SAS token.
        /// </summary>
        /// <param name="fileName">Name of the file for the URL generation.</param>
        /// <param name="accessKey">Access key to the storage (provided by Azure).</param>
        /// <returns>One-hour URL for the file.</returns>
        /// <exception cref="ArgumentException">Thrown when method arguments either null or invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when provided access token doesn't give the permission
        /// to the Azure storage.</exception>
        public string GetFileUrl(string? fileName, string? accessKey)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is null, empty or whitespace", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("Access key is null, empty or whitespace", nameof(accessKey));
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

        /// <summary>
        /// Provides needed metadata of the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="metadataName">Name of the needed metadata.</param>
        /// <returns>Needed metadata.</returns>
        /// <exception cref="ArgumentException">Thrown when method arguments either null or invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if metadata was not found.</exception>
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

            IDictionary<string, string> metadata = _blobStorage.GetBlobMetadata(fileName);

            string output;
            try
            {
                output =  metadata[metadataName];
            }
            catch(KeyNotFoundException){
                throw new InvalidOperationException("Specified metadata value doesn't exist");
            }

            return output;
        }
    }
}
