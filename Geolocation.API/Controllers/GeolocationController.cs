using Geolocation.Api.DTOs;
using Geolocation.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Geolocation.API.Controllers
{
    [Consumes("application/json")]
    public class GeolocationController : Controller
    {
        private readonly IGeolocationService _geolocationService;
        private readonly IIPStackService _ipStackService;
        private readonly ILogger<GeolocationController> _logger;
        public GeolocationController(IGeolocationService geolocationService, IIPStackService iPStackService, ILogger<GeolocationController> logger)
        {
            _geolocationService = geolocationService;
            _ipStackService = iPStackService;
            _logger = logger;
        }
        // GET: api/location/{id}
        [HttpGet("api/location/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var locationDto = await _geolocationService.GetById(id);
            return Ok(locationDto);
        }

        // GET: api/location/
        [HttpGet("api/location/getCoordinates/{ipAddress}")]
        public async Task<IActionResult> GetCoordinates([FromRoute] string ipAddress)
        {
            var locationDto = await _ipStackService.GetLocationDetails(ipAddress);
            return Ok(locationDto);
        }

        // POST: api/location/add
        [HttpPost("api/location/add")]
        public async Task<IActionResult> AddLocation([FromBody] AddLocationDto addLocationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var locationId = await _geolocationService.AddLocation(addLocationDto);
            return Created($"/api/location/{locationId}", null);
        }

        // DELETE: api/location/delete
        [HttpDelete("api/location/delete/{id}")]
        public async Task<IActionResult> DeleteLocation([FromRoute] int id)
        {
            var locationId = await _geolocationService.DeleteLocation(id);
            return Ok();
        }
    }
}
