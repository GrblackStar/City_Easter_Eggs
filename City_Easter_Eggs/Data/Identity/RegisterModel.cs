﻿#region Using

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AspNetCore.ReCaptcha;
using City_Easter_Eggs.Controllers;
using City_Easter_Eggs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace City_Easter_Eggs.Data.Identity
{
    [ValidateReCaptcha]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterInputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        private readonly UserService _userManager;

        public RegisterModel(UserService userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
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
                (IdentityResult result, User? user) = await _userManager.CreateAsync(Input.Username, Input.Password);
                if (!result.Succeeded || user == null)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }

                await _userManager.SignInAsync(user);
                return LocalRedirect(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }

    public class RegisterInputModel
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

        [DataType(DataType.Password)]
        [Display(Name = "Потвърди парола")]
        [Compare("Password", ErrorMessage = "Паролата и потвърдителната парола не съвпадат.")]
        public string? ConfirmPassword { get; set; }
    }
}