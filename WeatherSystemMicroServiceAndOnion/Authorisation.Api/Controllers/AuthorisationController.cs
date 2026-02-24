using Authorisation.Application.DTO;
using Authorisation.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authorisation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorisationController : ControllerBase
    {
        private readonly ILocalAuthServices localAuthService;
        private readonly ITokenService tokenService;
        private readonly IExternalAuthService externalAuthService;
        readonly ILogger<AuthorisationController> logger;

        public AuthorisationController(ILocalAuthServices _localAuthService, ITokenService _tokenService, IExternalAuthService _externalAuthService, ILogger<AuthorisationController> _logger)
        {
            localAuthService = _localAuthService;
            tokenService = _tokenService;
            externalAuthService = _externalAuthService;
            logger = _logger;
        }

        //Local Login
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto request)
        {

            if (!localAuthService.ValidateLocalUser(request.Username!, request.Password!))
            {
                logger.LogInformation($"Failed login attempt username: {request.Username}");
                return Unauthorized(new { message = "Invalid username or Password" });
            }

            var token = tokenService.GenerateToken(request.Username!);
            logger.LogInformation($"User {request.Username} logged in successfully");
            return Ok(
                new LoginResponse
                {
                    Token = token,
                    Username = request.Username!
                }
            );
        }


        //Local Register
        [HttpPost("register")]
        public IActionResult Register(RegisterRequestDto request)
        {
            if (!localAuthService.RegisterLocalUser(request.Username!, request.Password!))
            {
                logger.LogInformation($"Failed registration attempt username: {request.Username}");
                return Conflict(new { message = "Username already exists" });
            }
            var token = tokenService.GenerateToken(request.Username!);

            logger.LogInformation($"New user registered: {request.Username}");
            return Ok(new LoginResponse
            {
                Token = token,
                Username = request.Username!
            });
        }


        //Google Login
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse), "Authorisation", null, Request.Scheme)
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var cookieData = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (cookieData?.Principal == null)
            {
                logger.LogError("Google auth failed - no principal found");
                return Redirect("http://localhost:4200/?error=google_auth_fail");
            }
            var email = cookieData.Principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                logger.LogError("Google auth failed - no email");
                return Redirect("http://localhost:4200/?error=no_email");
            }

            externalAuthService.RegisterExternalUser(email!);

            var token = tokenService.GenerateToken(email!);

            logger.LogInformation($"Google login successfull for: {email}");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect($"http://localhost:4200/?token={token}&username={Uri.EscapeDataString(email)}");
        }


        //Facebook Login
        [HttpGet("facebook-login")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(FacebookResponse), "Authorisation", null, Request.Scheme)
            };

            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var cookieData = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (cookieData?.Principal == null)
            {
                logger.LogError("Google auth failed - no principal found");
                return Redirect("http://localhost:4200/?error=facebook_auth_fail");
            }

            var email = cookieData.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                logger.LogError("Facebook auth failed - no email");

                return Redirect("http://localhost:4200/?error=no_email");
            }

            externalAuthService.RegisterExternalUser(email!);

            var token = tokenService.GenerateToken(email!);

            logger.LogInformation($"Facebook login successfull for: {email}");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect($"http://localhost:4200/?token={token}&username={Uri.EscapeDataString(email)}");
        }
    }
}
