using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


///used for user registration request data transfer object
namespace Authorisation.Application.DTO
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
