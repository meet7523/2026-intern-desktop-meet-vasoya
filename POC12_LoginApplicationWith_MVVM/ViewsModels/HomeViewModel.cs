using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC12_LoginApplicationWith_MVVM.ViewsModels
{
    public class HomeViewModel
    {
        public string Username { get; }

        public HomeViewModel(string username)
        {
            Username = username;
        }
    }
}
