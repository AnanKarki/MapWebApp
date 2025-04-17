using Microsoft.AspNetCore.Mvc;
using MapWebApp.Server.Services;

namespace AzureMapWebApi.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureMapsController : ControllerBase
    {
        private readonly IAzureMapService _azureMapService;
        public AzureMapsController(IAzureMapService azureMapService)
        {
            _azureMapService = azureMapService;
        }
        [HttpGet("getdistricts")]
        public async Task<IActionResult> GetDistricts()
        {
            try
            {

                var districts = await _azureMapService.GetDistrictsAsync();
                return Ok(districts);
            }
            catch (Exception ex)
            {
                return NotFound("No features found.");

            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaces([FromQuery] string name)
        {
            try
            {

                var localdistrict = await _azureMapService.SearchPlaces(name);
                return Ok(localdistrict);
            }
            catch (Exception ex)
            {
                return NotFound("No features found.");

            }
        }
        [HttpGet("getpolygonbyreference")]
        public async Task<IActionResult> GetPolygonByReference([FromQuery] string reference)
        {
            try
            {
                var polygon = await _azureMapService.GetPolygonByReference(reference);
                return Ok(polygon);
            }
            catch (Exception ex)
            {
                return NotFound("No features found.");
            }
        }

        [HttpGet("uk-boundaries")]
        public async Task<IActionResult> GetLocalBoundaries()
        {
            try
            {
                var allboundaries = await _azureMapService.GetAllLocalBoundaries();
                return Content(allboundaries, "application/json"); ;

            }
            catch (Exception ex)
            {
                return NotFound("No features found.");

            }
        }
    }
}
