#region Using

using System.Security.Claims;
using City_Easter_Eggs.Data;
using City_Easter_Eggs.Helpers;
using City_Easter_Eggs.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#endregion

namespace City_Easter_Eggs.Controllers
{
    public class UserService
    {
        private ApplicationDbContext _db;
        private PasswordHasher _hasher;
        private IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _hasher = new PasswordHasher();
        }

        public async Task<(IdentityResult, User?)> CreateAsync(string username, string password)
        {
            if (username == null! || password == null!) return (IdentityResult.Failed(), null);

            IdentityResult validUsername = IsValidUsername(username);
            if (!validUsername.Succeeded) return (validUsername, null);

            IdentityResult validPassword = IsValidPassword(password);
            if (!validPassword.Succeeded) return (validPassword, null);

            var user = new User(username);
            user.PasswordHash = _hasher.HashPassword(password);
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return (IdentityResult.Success, user);
        }

        public async Task<(IdentityResult, User?)> VerifyUserSignIn(string username, string password)
        {
            if (username == null! || password == null!)
                return (IdentityResult.Failed(), null);

            string usernameNormalized = username.ToUpperInvariant();
            User? userFound = await _db.Users.FirstOrDefaultAsync(x => x.UsernameNormalized == usernameNormalized);
            if (userFound == null)
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "NoSuchUser",
                    Description = "Invalid username."
                }), null);

            PasswordVerificationResult passwordCheckResult = _hasher.VerifyHashedPassword(userFound.PasswordHash ?? "", password);
            if (passwordCheckResult == PasswordVerificationResult.Failed)
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "WrongPassword",
                    Description = "Wrong password."
                }), null);

            return (IdentityResult.Success, userFound);
        }

        private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

        // These constraints should match the frontend ones in RegisterInputModel for optimal(tm) experience.
        // todo: reference constants there
        private const int RequiredLength = 6;
        private const int UsernameMaxLength = 25;

        private IdentityResult IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < RequiredLength || username.Length > UsernameMaxLength)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameLength",
                    Description = $"Username needs to be between {RequiredLength} and {UsernameMaxLength} characters long."
                });

            if (username.Any(x => AllowedCharacters.IndexOf(x) == -1))
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameInvalidCharacters",
                    Description = "Your username can only contain alphanumeric characters."
                });

            string normalizedUserName = username.ToUpperInvariant();
            if (_db.Users.Any(x => x.UsernameNormalized == normalizedUserName))
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameTaken",
                    Description = "There is already a user with that username."
                });

            return IdentityResult.Success;
        }

        private IdentityResult IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < RequiredLength || password.Length > 100)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "PasswordLength",
                    Description = $"Password needs to be between {RequiredLength} and 100 characters long."
                });

            return IdentityResult.Success;
        }

        #region Login Logic

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return principal.Identities.Any(i => i.AuthenticationType == IdentityConstants.ApplicationScheme);
        }

        public ClaimsPrincipal CreateUserPrincipal(User user)
        {
            string userId = user.UserId;
            string userName = user.UsernameNormalized;
            var id = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            id.AddClaim(new Claim(ClaimTypes.Name, userName));

            return new ClaimsPrincipal(id);
        }

        public async Task<User?> GetUserFromPrincipal(ClaimsPrincipal principal)
        {
            string? id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return null;
            return await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public string? GetUserIdFromPrinciple(ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public Task<User?> GetUserFromId(string userId)
        {
            return _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task SignInAsync(User user, bool rememberMe = false)
        {
            ClaimsPrincipal principal = CreateUserPrincipal(user);

            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            // Ensure clean login
            await SignOutAsync();
            await httpContext.SignInAsync(principal, new AuthenticationProperties {IsPersistent = rememberMe});
            httpContext.User = principal;
        }

        public async Task SignOutAsync()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        }

        #endregion
    }
}