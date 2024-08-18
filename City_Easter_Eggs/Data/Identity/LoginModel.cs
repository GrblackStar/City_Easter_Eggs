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

        private readonly UserService _userManager;

        public LoginModel(UserService userManager)
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
			        ModelState.AddModelError(string.Empty, "reCAPTCHA проверката не бе успешна - опитай отново, human...");
			        return Page();
		        }
	        }

            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                (IdentityResult result, User? user) = await _userManager.VerifyUserSignIn(Input.Username, Input.Password);
                if (!result.Succeeded || user == null)
                {
                    ModelState.AddModelError(string.Empty, "Невалиден опит.");
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
        [Required(ErrorMessage = "Това поле е задължително!")]
        [StringLength(25, ErrorMessage = "Името трябва да бъде най-малко {2} и най-много {1} символа.", MinimumLength = 6)]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Това поле е задължително!")]
        [StringLength(100, ErrorMessage = "Паролата трябва да бъде най-малко {2} и най-много {1} символа.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = default!;

        [Display(Name = "Запомни ме?")]
        public bool RememberMe { get; set; } = default!;
    }
}