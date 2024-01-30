using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace DNAiWeb.Components.Account.Pages
{
    // SigninGoogle.cshtml.cs
    public class SigninGoogleModel : PageModel
    {
        private readonly IGoogleAuthProvider _googleAuthProvider;

        public SigninGoogleModel(IGoogleAuthProvider googleAuthProvider)
        {
            _googleAuthProvider = googleAuthProvider;
        }
        public interface IGoogleAuthProvider
        {
            Task<OAuthTokenResponse> ExchangeCodeForTokensAsync(string code);
        }


        public async Task<IActionResult> OnGetCallbackAsync()
        {
            try
            {
                // 1. Extract authentication information:
                var state = await HttpContext.GetTokenAsync("state");
                var code = await HttpContext.GetTokenAsync("code");

                // 2. Validate state (if applicable):
                // ...

                // 3. Exchange code for tokens:
                var tokens = await _googleAuthProvider.ExchangeCodeForTokensAsync(code);

                // 4. Create user identity:
                var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, tokens.AccessToken),
    // Add other claims as needed
};

                var userIdentity = new ClaimsIdentity(claims, GoogleDefaults.AuthenticationScheme);

                // 5. Create ClaimsPrincipal:
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                // 6. Sign in user:
                await HttpContext.SignInAsync(GoogleDefaults.AuthenticationScheme, userPrincipal);

                return RedirectToPage("/Pages/Home");
            }
            catch (Exception ex)
            {
                // Handle authentication errors gracefully
                // Log the error and potentially redirect to an error page
                ModelState.AddModelError("", "Authentication failed. Please try again.");
                return Page(); // Redisplay the page with an error message
            }
        }
    }

}
