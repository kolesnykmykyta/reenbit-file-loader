﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BlobAccess
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly BlobContainerClient _containerClient;

        public BlobStorage(string connectionString, string containerName)
        {
            _serviceClient = new BlobServiceClient(connectionString);
            _containerClient = _serviceClient.GetBlobContainerClient(containerName);
        }

        public async Task UploadFileAsync(Stream file, string fileName, Dictionary<string, string>? metadata = null)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            BlobUploadOptions options = new BlobUploadOptions
            {
                Metadata = metadata,
            };

            await blobClient.UploadAsync(file, options);
        }
    }
}