#region Using

using System.Security.Claims;
using City_Easter_Eggs.Controllers;
using City_Easter_Eggs.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

#endregion

namespace City_Easter_Eggs.Helpers;

// Copy from ASPNetCore.Identity with support for custom user controller.
public class SecurityStampValidator : ISecurityStampValidator
{
    private UserController _userController;
    private SecurityStampValidatorOptions _options;
    private TimeProvider _timeProvider;

    public SecurityStampValidator(UserController userController)
    {
        _userController = userController;
        _options = new SecurityStampValidatorOptions();
        _timeProvider = TimeProvider.System;
    }

    protected virtual async Task SecurityStampVerified(User user, CookieValidatePrincipalContext context)
    {
        ClaimsPrincipal newPrincipal = _userController.CreateUserPrincipal(user);

        if (_options.OnRefreshingPrincipal != null)
        {
            var replaceContext = new SecurityStampRefreshingPrincipalContext
            {
                CurrentPrincipal = context.Principal,
                NewPrincipal = newPrincipal
            };

            // Note: a null principal is allowed and results in a failed authentication.
            await _options.OnRefreshingPrincipal(replaceContext);
            newPrincipal = replaceContext.NewPrincipal;
        }

        // REVIEW: note we lost login authentication method
        context.ReplacePrincipal(newPrincipal);
        context.ShouldRenew = true;

        if (!context.Options.SlidingExpiration)
            // On renewal calculate the new ticket length relative to now to avoid
            // extending the expiration.
            context.Properties.IssuedUtc = _timeProvider.GetUtcNow();
    }

    protected virtual Task<User> VerifySecurityStamp(ClaimsPrincipal? principal)
    {
        return _userController.GetUserFromPrincipal(principal);
    }

    public virtual async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        DateTimeOffset currentUtc = _timeProvider.GetUtcNow();
        DateTimeOffset? issuedUtc = context.Properties.IssuedUtc;

        // Only validate if enough time has elapsed
        bool validate = issuedUtc == null;
        if (issuedUtc != null)
        {
            TimeSpan timeElapsed = currentUtc.Subtract(issuedUtc.Value);
            validate = timeElapsed > _options.ValidationInterval;
        }

        if (validate)
        {
            User? user = await VerifySecurityStamp(context.Principal);
            if (user != null)
            {
                ClaimsPrincipal principal = _userController.CreateUserPrincipal(user);
                context.ReplacePrincipal(principal);
                context.ShouldRenew = true;
            }
            else
            {
                context.RejectPrincipal();
                await _userController.SignOutAsync();
            }
        }
    }
}