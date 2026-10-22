using POC12__TaskManagementApp.Services;
using POC15__TaskManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC15__TaskManagementApp.Services
{
    public class NavigationServices : INavigationServices
    {
        private BaseViewModel _currentView;

        public BaseViewModel CurrentView
        {
            get { return _currentView; }
            private set
            {
                _currentView = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentView)));
            }
        }

        public void Navigate(BaseViewModel viewModel)
        {
            CurrentView = viewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
