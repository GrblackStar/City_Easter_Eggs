#region Using

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AspNetCore.ReCaptcha;
using City_Easter_Eggs.Controllers;
using City_Easter_Eggs.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace City_Easter_Eggs.Data.Identity
{
    [ValidateReCaptcha]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        private readonly UserController _userManager;

        public LoginModel(UserController userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Clear the existing external cookie to ensure a clean login process
            await _userManager.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
	        if (!ModelState.IsValid)
	        {
		        ModelStateEntry? recaptchaState = ModelState["Recaptcha"];
		        if (recaptchaState != null && recaptchaState.Errors.Count > 0)
		        {
			        ModelState.AddModelError(string.Empty, "Recaptcha check failed - please try again, human.");
			        return Page();
		        }
	        }

            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                (IdentityResult result, User? user) = await _userManager.VerifyUserSignIn(Input.Username, Input.Password);
                if (!result.Succeeded || user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                await _userManager.SignInAsync(user, Input.RememberMe);
                return LocalRedirect(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }

    public class LoginInputModel
    {
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Username")]
        public string Username { get; set; } = default!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = default!;
    }
}