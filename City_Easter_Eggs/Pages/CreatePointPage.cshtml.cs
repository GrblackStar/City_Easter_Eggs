using City_Easter_Eggs.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace City_Easter_Eggs.Pages
{
    public class CreatePointPageModel : PageModel
    {
        [BindProperty]
        public CreatePointInputModel Input { get; set; } = default!;
        public string? ReturnUrl { get; set; }
        private readonly PointsService _pointService;

        public CreatePointPageModel(PointsService pointService)
        {
            _pointService = pointService;
        }

        public async Task OnGetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _pointService.CreatePointAsync(Input.Name, Input.Description, Input.UserLocationLongitude, Input.UserLocationLatitude);

            return RedirectToPage("/Index");
        }
    }

    public class CreatePointInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 0)]
        public string? Description { get; set; }

        public double UserLocationLongitude { get; set; }
        public double UserLocationLatitude { get; set; }
    }

}
