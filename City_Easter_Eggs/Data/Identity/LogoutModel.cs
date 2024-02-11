#region Using

using City_Easter_Eggs.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace City_Easter_Eggs.Data.Identity
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly UserService _userManager;

        public LogoutModel(UserService userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            await _userManager.SignOutAsync();

            if (returnUrl != null)
                return LocalRedirect(returnUrl);

            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToPage();
        }
    }
}