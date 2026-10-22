using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.Services
{
    public class FontSettingsService : IFontSettingsService
    {
        private double _fontSize;

        public event PropertyChangedEventHandler PropertyChanged;

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public FontSettingsService()
        {
            Load();
        }

        public void Load()
        {
            FontSize = Properties.Settings.Default.FontSize;
        }

        public void Save()
        {
            Properties.Settings.Default.FontSize = FontSize;
            Properties.Settings.Default.Save();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

