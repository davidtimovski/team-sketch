using Microsoft.AspNetCore.Mvc;
using TeamSketch.Web.Services;

namespace TeamSketch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILiveLocationsService _liveLocationsService;

        public LocationsController(ILiveLocationsService liveLocationsService)
        {
            _liveLocationsService = liveLocationsService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var locations = _liveLocationsService.GetAll();
            return Ok(locations);
        }
    }
}
