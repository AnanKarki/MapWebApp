namespace MapWebApp.Server.Services
{
    public interface IFileServices
    {
        Task<string> LoadLocalAuthorityDistrict();
    }
    /// <summary>
    /// This class provides methods to load and manage files.
    /// </summary>
    public class FileServices : IFileServices
    {
        private readonly IWebHostEnvironment _env;

        private readonly string _fileName = "data/local-authority-district.geojson";
        public FileServices(IWebHostEnvironment env)
        {
            _env = env;

        }
        public Task<string> LoadLocalAuthorityDistrict()
        {
            var filePath = Path.Combine(_env.WebRootPath, _fileName);

            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException("File does not exist");

            var json = System.IO.File.ReadAllTextAsync(filePath);
            return json;
        }

    }
}
