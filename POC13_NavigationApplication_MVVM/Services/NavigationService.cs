using POC13_NavigationApplication_MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.Services
{
    public class NavigationService : INavigationService
    {
        private readonly MainViewModel mainViewModel;

        public BaseViewModel CurrentView { get; private set; }

        public NavigationService(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public void NavigateTo(BaseViewModel viewModel)
        {
            mainViewModel.CurrentViewModel = viewModel;
        }

        public void Navigate(BaseViewModel viewModel)
        {
            CurrentView = viewModel;
        }
    }
}
