using MapWebApp.Server.Model;
using System.Text.Json.Nodes;

namespace MapWebApp.Server.Services
{
    /// <summary>
    /// This interface defines methods for interacting with Azure Maps services.
    /// which includes retrieving district data, searching for places, and getting polygons by reference.
    /// </summary>
    public interface IAzureMapService
    {
        Task<List<District>> GetDistrictsAsync();
        Task<JsonNode> GetPolygonByReference(string reference);
        Task<string> GetAllLocalBoundaries();
        Task<List<JsonNode>> SearchPlaces(string name);
    }
    /// <summary>
    /// This class implements the IAzureMapService interface and provides methods to interact with Azure Maps services.

    /// </summary>
    public class AzureMapService : IAzureMapService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFileServices _fileServices;
        public AzureMapService(IWebHostEnvironment env, IFileServices fileServices) 
        {
            _env = env;
            _fileServices = fileServices;
        }

        public async Task<string> GetAllLocalBoundaries()
        {
            var json = await _fileServices.LoadLocalAuthorityDistrict();
            return json;
        }

        public async Task<List<District>> GetDistrictsAsync()
        {
            JsonNode? root = JsonNode.Parse(await _fileServices.LoadLocalAuthorityDistrict());

            var features = root?["features"]?.AsArray();

            if (features == null)
                throw new ArgumentException("Feature Not Found");

            var districts = features
                .Select(f => new District
                {
                    Name = f?["properties"]?["name"]?.ToString() ?? string.Empty,
                    Reference = f?["properties"]?["reference"]?.ToString() ?? string.Empty
                })
                .Where(item => !string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Reference))
                .ToList();

            return districts;
        }

        public async Task<JsonNode> GetPolygonByReference(string reference)
        {
            var data = await _fileServices.LoadLocalAuthorityDistrict();
            JsonNode? root = JsonNode.Parse(data);

            var features = root?["features"]?.AsArray();

            if (features == null)
                throw new ArgumentException("Feature Not Found");

            var match = features.FirstOrDefault(f =>
             f?["properties"]?["reference"]?.ToString() == reference
             );
            var geometry = match?["geometry"];

            return geometry;
        }

        public async Task<List<JsonNode>> SearchPlaces(string name)
        {
            var geoJsonText = await _fileServices.LoadLocalAuthorityDistrict();
            JsonNode? root = JsonNode.Parse(geoJsonText);

            var features = root?["features"]?.AsArray();

            if (features != null)
            {
                var matches = features
                    .Where(f => f?["properties"]?["name"] != null &&
                                f["properties"]["name"]!.ToString().ToLower().Contains(name.ToLower()))
                    .ToList();

                return matches;
            }
            // If no matches found, return an empty list
            throw new ArgumentException("Feature not found");
        }
    }
}
