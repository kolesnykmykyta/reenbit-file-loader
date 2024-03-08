using Infrastructure.BlobAccess;
using Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class BlobStorageServiceTests
    {
        private readonly Mock<Stream> mockStream;
        public BlobStorageServiceTests()
        {
            mockStream = new Mock<Stream>();
            mockStream.Setup(x => x.Length).Returns(512);
        }

        [Theory]
        [InlineData("test.docx")]
        [InlineData("test.DOCX")]
        [InlineData("test.pdf.docx")]
        [InlineData("test.DocX")]
        [InlineData("test.pdf.doCX")]
        [InlineData(".docx")]
        public async Task UploadFileToStorageAsync_ValidInputs_UploadsTheFile(string originalName)
        {
            var blobStorageMock = new Mock<IBlobStorage>();
            BlobStorageService service = new BlobStorageService(blobStorageMock.Object);

            await service.UploadFileToStorageAsync(mockStream.Object, originalName, "myemail@gmail.com");

            blobStorageMock.Verify(x =>
                x.UploadFileAsync(
                    It.IsNotNull<Stream>(),
                    It.Is<string>(s => !s.Equals(originalName)),
                    It.Is<Dictionary<string, string>>(d => d.Keys.Any(key => key == "email"))),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task UploadFileToStorageAsync_InvalidFileName_ThrowsArgumentException(string? originalName)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "originalName";

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.UploadFileToStorageAsync(mockStream.Object, originalName, "test@email"));

            Assert.Equal(expectedParamName, exception.ParamName);
        }

        [Theory]
        [InlineData("test.pdf")]
        [InlineData(".")]
        [InlineData("test.docx.pdf")]
        [InlineData("test.doc")]
        public async Task UploadFileToStorageAsync_InvalidFileExtension_ThrowsArgumentException(string originalName)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "file";

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.UploadFileToStorageAsync(mockStream.Object, originalName, "test@email"));

            Assert.Equal(expectedParamName, exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task UploadFileToStorageAsync_InvalidEmail_ThrowsArgumentException(string? email)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "email";

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.UploadFileToStorageAsync(mockStream.Object, "test.docx", email));

            Assert.Equal(expectedParamName, exception.ParamName);
        }

        [Fact]
        public async Task UploadFileToStorageAsync_NullFile_ThrowsArgumentNullException()
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "file";

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => service.UploadFileToStorageAsync(null, "test.docx", "test@email"));

            Assert.Equal(expectedParamName, exception.ParamName);
        }
    }
}
