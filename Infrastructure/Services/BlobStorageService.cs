using Infrastructure.BlobAccess;
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
            if (file == null || file.Length == 0)
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
            if (!Path.GetExtension(originalName).ToLower().Equals(".docx")){
                throw new ArgumentException("File extension is not .docx", nameof(file));
            }

            string newName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}_{originalName}";
            Dictionary<string, string> metadata = new Dictionary<string, string>
            {
                {"email", email },
            };

            await _blobStorage.UploadFileAsync(file, newName, metadata);
        }
    }
}
