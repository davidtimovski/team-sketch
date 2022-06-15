using Microsoft.AspNetCore.Mvc;
using TeamSketch.Web.Services;

namespace TeamSketch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveViewController : ControllerBase
    {
        private readonly ILiveViewService _liveLiveService;

        public LiveViewController(ILiveViewService liveViewService)
        {
            _liveLiveService = liveViewService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var locations = _liveLiveService.GetLocations();
            return Ok(locations);
        }
    }
}
