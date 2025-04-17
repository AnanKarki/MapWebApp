using Microsoft.AspNetCore.Hosting;
using Moq;
using MapWebApp.Server.Services;

namespace AzureMapWebAppTestProject
{
    public class FileServicesTests
    {
        private readonly string _tempDirectory;
        private readonly string _testFilePath;

        public FileServicesTests()
        {
            // Setup temp directory and file
            _tempDirectory = Path.Combine(Path.GetTempPath(), "FileServicesTest_" + Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDirectory);

            _testFilePath = Path.Combine(_tempDirectory, "data", "local-authority-district.geojson");
            Directory.CreateDirectory(Path.GetDirectoryName(_testFilePath)!);

            File.WriteAllText(_testFilePath, "{\"features\":[]}");
        }

        [Fact]
        public async Task LoadLocalAuthorityDistrict_ReturnsFileContents()
        {
            // Arrange
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(env => env.WebRootPath).Returns(_tempDirectory);

            var fileService = new FileServices(mockEnv.Object);

            // Act
            var result = await fileService.LoadLocalAuthorityDistrict();

            // Assert
            Assert.Equal("{\"features\":[]}", result);
        }

        [Fact]
        public async Task LoadLocalAuthorityDistrict_FileMissing_ThrowsFileNotFoundException()
        {
            // Arrange
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(env => env.WebRootPath).Returns(Path.Combine(_tempDirectory, "missing"));

            var fileService = new FileServices(mockEnv.Object);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => fileService.LoadLocalAuthorityDistrict());
        }

       
    }
}
