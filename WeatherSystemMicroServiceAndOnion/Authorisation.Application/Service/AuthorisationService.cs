using Authorisation.Application.Interfaces;
using Authorisation.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authorisation.Application.Service
{
    public class AuthorisationService : ILocalAuthServices, ITokenService, IExternalAuthService
    {
        private readonly IConfiguration config;
        private readonly IUserRepository userRepo;

        public AuthorisationService(IConfiguration _config, IUserRepository _userRepo)
        {
            config = _config;
            userRepo = _userRepo;
        }

        public static string HashPassword(string password)
        {
            var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool ValidateLocalUser(string username, string password)
        {
            var storedHash = userRepo.GetPasswordHash(username);
            if (storedHash == null) return false;
            return storedHash == HashPassword(password);
        }

        public bool RegisterLocalUser(string username, string password)
        {
            if (userRepo.UserExist(username)) return false;
            userRepo.AddUser(username, HashPassword(password));
            return true;
        }
        
        public void RegisterExternalUser(string email)
        {
            if (!userRepo.UserExist(email))
            {
                userRepo.AddUser(email, string.Empty, true);
            }
        }

        public string GenerateToken(string username)
        {
            var jwtSettings = config.GetSection("JWTSetting");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken
            (
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
