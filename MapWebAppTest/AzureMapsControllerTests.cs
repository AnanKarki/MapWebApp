using Microsoft.AspNetCore.Mvc;
using Moq;
using MapWebApp.Server.Controllers;
using MapWebApp.Server.Model;
using MapWebApp.Server.Services;
using System.Text.Json.Nodes;

namespace AzureMapWebAppTestProject
{
    public class AzureMapsControllerTests
    {
        private readonly Mock<IAzureMapService> _mockService;
        private readonly AzureMapsController _controller;

        public AzureMapsControllerTests()
        {
            _mockService = new Mock<IAzureMapService>();
            _controller = new AzureMapsController(_mockService.Object);
        }

        [Fact]
        public async Task GetDistricts_ReturnsOk_WithData()
        {
            // Arrange
            var expected = new List<District> { new District() { Name = "Halton", Reference ="EB00001"},
                new District() { Name = "London", Reference ="EB00002"} };
            _mockService.Setup(s => s.GetDistrictsAsync()).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetDistricts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetDistricts_ReturnsNotFound_OnError()
        {
            _mockService.Setup(s => s.GetDistrictsAsync()).ThrowsAsync(new Exception("Error"));

            var result = await _controller.GetDistricts();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SearchPlaces_ReturnsOk_WithData()
        {
            var expected = new List<JsonNode>();

            var feature = new JsonObject
            {
                ["type"] = "Feature",
                ["geometry"] = new JsonObject
                {
                    ["type"] = "Point",
                    ["coordinates"] = new JsonArray { -0.1278, 51.5074 }
                },
                ["properties"] = new JsonObject
                {
                    ["name"] = "London"
                }
            };
            expected.Add(feature);

            _mockService.Setup(s => s.SearchPlaces("test")).ReturnsAsync(expected);
            var result = await _controller.SearchPlaces("test");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetPolygonByReference_ReturnsOk_WithData()
        {
            var pologon = new JsonObject()
            {
                ["type"] = "FeatureCollection",
                ["geometry"] = new JsonObject
                {
                    ["type"] = "Point",
                    ["coordinates"] = new JsonArray { -0.1278, 51.5074 }
                },
            };
            _mockService.Setup(s => s.GetPolygonByReference("abc123")).ReturnsAsync(pologon);

            var result = await _controller.GetPolygonByReference("abc123");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(pologon, okResult.Value);
        }

        [Fact]
        public async Task GetLocalBoundaries_ReturnsContent_WithJson()
        {
            var json = "{ \"type\": \"FeatureCollection\", \"features\": [] }";
            _mockService.Setup(s => s.GetAllLocalBoundaries()).ReturnsAsync(json);

            var result = await _controller.GetLocalBoundaries();

            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", contentResult.ContentType);
            Assert.Equal(json, contentResult.Content);
        }
    }
}
