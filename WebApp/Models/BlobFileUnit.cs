using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Models
{
    public class BlobFileUnit
    {
        public IBrowserFile? File { get; set; }

        public string? Email { get; set; }
    }
}
