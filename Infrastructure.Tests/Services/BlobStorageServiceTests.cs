﻿using Azure;
using Azure.Storage.Blobs.Models;
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
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
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
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
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
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Fact]
        public async Task UploadFileToStorageAsync_NullFile_ThrowsArgumentNullException()
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "file";

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => service.UploadFileToStorageAsync(null, "test.docx", "test@email"));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Fact]
        public void GetFileUrl_CorrectValues_ReturnsFileUrl()
        {
            string fileName = "test.docx";
            string accessKey = "access-key";
            string storageName = "storage-name";
            string containerName = "container-name";
            string sasToken = "sas-token";
            var blobStorageMock = new Mock<IBlobStorage>();
            blobStorageMock.Setup(x => x.ContainerName)
                .Returns(containerName);
            blobStorageMock.Setup(x => x.StorageName)
                .Returns(storageName);
            blobStorageMock.Setup(x => x.GenerateSasToken(fileName, accessKey, 3600))
                .Returns(sasToken);
            string expected = $"https://{storageName}.blob.core.windows.net/{containerName}/{fileName}?{sasToken}";
            BlobStorageService service = new BlobStorageService(blobStorageMock.Object);

            string actual = service.GetFileUrl(fileName, accessKey);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetFileUrl_InvalidFileName_ThrowsArgumentException(string? fileName)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "fileName";

            var exception = Assert.Throws<ArgumentException>(() => service.GetFileUrl(fileName, "access-key"));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetFileUrl_InvalidAccessKey_ThrowsArgumentException(string? accessKey)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "accessKey";

            var exception = Assert.Throws<ArgumentException>(() => service.GetFileUrl("test.docx", accessKey));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Fact]
        public void GetFileUrl_WrongAccessKey_ThrowsInvalidOperationException()
        {
            var blobStorageMock = new Mock<IBlobStorage>();
            blobStorageMock.Setup(x => x.GenerateSasToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new RequestFailedException("Error Message"));

            BlobStorageService service = new BlobStorageService(blobStorageMock.Object);

            Assert.Throws<InvalidOperationException>(() => service.GetFileUrl("text.docx", "invalidkey"));
        }

        [Fact]
        public void GetBlobMetadata_CorrectValues_ReturnsMetadata()
        {
            string metadataKey = "key";
            string expected = "value";
            var blobStorageMock = new Mock<IBlobStorage>();
            blobStorageMock.Setup(x => x.GetBlobMetadata(It.IsAny<string>()))
                .Returns(new Dictionary<string, string>() { { metadataKey, expected } });
            BlobStorageService service = new BlobStorageService(blobStorageMock.Object);

            string actual = service.GetBlobMetadata("test.json", metadataKey);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void GetBlobMetadata_InvalidFileName_ThrowsArgumentException(string? fileName)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "fileName";

            var exception = Assert.Throws<ArgumentException>(() => service.GetBlobMetadata(fileName, "metadata"));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void GetBlobMetadata_InvalidMetadataName_ThrowsArgumentException(string? metadataName)
        {
            BlobStorageService service = new BlobStorageService(new Mock<IBlobStorage>().Object);
            string expectedParamName = "metadataName";

            var exception = Assert.Throws<ArgumentException>(() => service.GetBlobMetadata("test.docx", metadataName));
            string? actualParamName = exception.ParamName;

            Assert.Equal(expectedParamName, actualParamName);
        }

        [Fact]
        public void GetBlobMetadata_NotExistingMetadata_ThrowsInvalidOperationException()
        {
            var blobStorageMock = new Mock<IBlobStorage>();
            blobStorageMock.Setup(x => x.GetBlobMetadata(It.IsAny<string>()))
               .Returns(new Dictionary<string, string>());
            BlobStorageService service = new BlobStorageService(blobStorageMock.Object);

            Assert.Throws<InvalidOperationException>(() => service.GetBlobMetadata("test.json", "example"));
        }
    }
}
