#region Using

using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace City_Easter_Eggs.Pages
{
    public class MapPageComponent : PageModel
    {
        private readonly ILogger<AboutModel> _logger;

        public MapPageComponent(ILogger<AboutModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}