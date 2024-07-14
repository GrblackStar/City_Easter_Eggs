#region Using

using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace City_Easter_Eggs.Pages
{
    public class AboutModel : PageModel
    {
        private readonly ILogger<AboutModel> _logger;

        public AboutModel(ILogger<AboutModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}