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

        public async Task UploadFileToStorageAsync(Stream file, string originalName, string email)
        {
            if (!Path.GetExtension(originalName).Equals(".docx")){
                throw new ArgumentException("File extension is not .docx", nameof(originalName));
            }
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            string newName = $"{Path.GetRandomFileName()}_{originalName}.docx";
            Dictionary<string, string> metadata = new Dictionary<string, string>
            {
                {"email", email },
            };

            await _blobStorage.UploadFileAsync(file, newName, metadata);
        }
    }
}
