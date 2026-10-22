using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC12_LoginApplicationWith_MVVM.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
    }
}
