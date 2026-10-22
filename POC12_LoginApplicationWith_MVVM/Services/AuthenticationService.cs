using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static POC12_LoginApplicationWith_MVVM.Services.AuthenticationService;

namespace POC12_LoginApplicationWith_MVVM.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public bool Authenticate(string username, string password)
        {
            return username == Credentials.uname && password == Credentials.password;
        }
    }
}


