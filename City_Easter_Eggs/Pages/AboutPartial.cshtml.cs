using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace City_Easter_Eggs.Pages
{
    public class AboutPartialModel : PageModel
    {
        private readonly ILogger<AboutModel> _logger;

        public AboutPartialModel(ILogger<AboutModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
