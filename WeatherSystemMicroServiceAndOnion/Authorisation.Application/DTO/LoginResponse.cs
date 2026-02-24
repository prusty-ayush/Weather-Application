///used for returning the token and username after successful login and registration also
namespace Authorisation.Application.DTO
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
    }
}
