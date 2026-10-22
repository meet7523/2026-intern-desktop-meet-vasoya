using POC13_NavigationApplication_MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.Services
{
    public interface INavigationService 
    {
        void NavigateTo(BaseViewModel viewModel);
        void Navigate(BaseViewModel viewModel);
    }
}
