using POC13_NavigationApplication_MVVM.Properties;
using POC13_NavigationApplication_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.ViewModels
{

    public class HomeViewModel : BaseViewModel
    {
        private readonly FontSettingsService _settings;

        public double FontSize => _settings.FontSize;
    

        public HomeViewModel()
        {
            _settings = App.SettingsService;

            _settings.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(FontSettingsService.FontSize))
                {
                    OnPropertyChanged(nameof(FontSize));
                }
            };
        }
    }
}


