using System;
using System.Collections.Generic;
using System.Text;

namespace Authorisation.Application.Interfaces
{
    public interface IExternalAuthService
    {
        public void RegisterExternalUser(string email);
    }
}
