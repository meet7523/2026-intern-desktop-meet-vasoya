using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.Services
{
    public interface IFontSettingsService
    {
        double FontSize { get; set; }

        void Load();

        void Save();
    }
}
