using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TeamSketch.Web.Pages
{
    public class IndexModel(IConfiguration configuration) : PageModel
    {
        public string BaseUrl { get; private set; } = configuration["BaseUrl"]!;
    }
}
