using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TeamSketch.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(IConfiguration configuration)
        {
            BaseUrl = configuration["BaseUrl"];
        }

        public string BaseUrl { get; private set; }
    }
}
