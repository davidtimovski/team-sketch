using Microsoft.AspNetCore.Mvc;
using TeamSketch.Web.Services;

namespace TeamSketch.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class LiveViewController : ControllerBase
{
    private readonly ILiveViewService _liveLiveService;

    public LiveViewController(ILiveViewService liveViewService)
    {
        _liveLiveService = liveViewService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var locations = _liveLiveService.GetDistinctLocations();
        return Ok(locations);
    }
}
