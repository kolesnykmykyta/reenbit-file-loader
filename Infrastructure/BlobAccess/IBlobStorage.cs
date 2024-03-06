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
    }
}
