using POC13_NavigationApplication_MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.Services
{
    public interface ISettingsService
    {
        UserSettings Settings { get; }

        void Load();

        void Save();
    }
}

