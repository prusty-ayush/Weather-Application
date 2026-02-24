using System;
using System.Collections.Generic;
using System.Text;

namespace Authorisation.Application.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(string username);
    }
}
