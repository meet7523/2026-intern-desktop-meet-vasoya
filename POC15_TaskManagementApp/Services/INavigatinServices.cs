using POC15__TaskManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC15__TaskManagementApp.Services
{
        public interface INavigationServices
        {
            void Navigate(BaseViewModel viewModel);
        }
    
}
