﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Interfaces
{
    public interface IBlobStorageService
    {
        public Task UploadFileToStorageAsync(Stream? file, string? originalName, string? email);

        public string GetFileUrl(string? fileName, string? accessKey);

        public string GetBlobMetadata(string? fileName, string? metadataName);
    }
}
