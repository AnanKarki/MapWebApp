using Microsoft.AspNetCore.Hosting;
using Moq;
using MapWebApp.Server.Services;

namespace AzureMapWebAppTestProject
{
    public class AzureMapServiceTests
    {
        private readonly Mock<IFileServices> _mockFileServices;
        private readonly AzureMapService _mapService;

        private const string SampleJson = @"
    {
        ""features"": [
            {
                ""properties"": {
                    ""name"": ""District One"",
                    ""reference"": ""D1""
                },
                ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[1, 2], [3, 4], [5, 6], [1, 2]]]}
            },
            {
                ""properties"": {
                    ""name"": ""District Two"",
                    ""reference"": ""D2""
                },
                ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[7, 8], [9, 10], [11, 12], [7, 8]]]}
            }
        ]
    }";

        public AzureMapServiceTests()
        {
            _mockFileServices = new Mock<IFileServices>();
            _mockFileServices.Setup(fs => fs.LoadLocalAuthorityDistrict())
                             .ReturnsAsync(SampleJson);

            // IWebHostEnvironment is unused in service logic
            var envMock = new Mock<IWebHostEnvironment>();
            _mapService = new AzureMapService(envMock.Object, _mockFileServices.Object);
        }

        [Fact]
        public async Task GetAllLocalBoundaries_ReturnsRawJson()
        {
            var result = await _mapService.GetAllLocalBoundaries();
            Assert.Equal(SampleJson, result);
        }

        [Fact]
        public async Task GetDistrictsAsync_ReturnsListOfDistricts()
        {
            var districts = await _mapService.GetDistrictsAsync();
            Assert.Equal(2, districts.Count);
            Assert.Contains(districts, d => d.Name == "District One" && d.Reference == "D1");
        }

        [Fact]
        public async Task GetPolygonByReference_ReturnsCorrectPolygon()
        {
            var result = await _mapService.GetPolygonByReference("D2");

            Assert.NotNull(result);
            Assert.Equal("Polygon", result?["type"]?.ToString());
        }

        [Fact]
        public async Task GetPolygonByReference_InvalidReference_ReturnsEmpty()
        {
            var result = await _mapService.GetPolygonByReference("INVALID");
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task SearchPlaces_ReturnsMatchingDistricts()
        {
            var result = await _mapService.SearchPlaces("one");
            Assert.Single(result);
            Assert.Equal("District One", result.First()?["properties"]?["name"]?.ToString());
        }

        [Fact]
        public async Task SearchPlaces_NoMatch_Count()
        {
            var result = await _mapService.SearchPlaces("Nowhere");
            Assert.True(result.Count == 0);
        }
    }
}
