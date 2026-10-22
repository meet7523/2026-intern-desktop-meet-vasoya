using POC13_NavigationApplication_MVVM.Models;
using POC13_NavigationApplication_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.ViewModels
{
    internal class ProductsViewModel : BaseViewModel
    {
        private SettingsService _settings;

        public ObservableCollection<Products> Products { get; }

        private double _fontSize;
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }


        public ProductsViewModel()
        {
            _settings = new SettingsService();

            FontSize = Properties.Settings.Default.FontSize;
            Products = new ObservableCollection<Products>
        {
            new Products
            {
                ProductName = "Laptop",
                Category = "Electronics",
                Qty = 10,
                Price = 65000
            },
            new Products
            {
                ProductName = "Mouse",
                Category = "Accessories",
                Qty = 50,
                Price = 500
            },
            new Products
            {
                ProductName = "Keyboard",
                Category = "Accessories",
                Qty = 25,
                Price = 1200
            },
            new Products
            {
                ProductName = "Monitor",
                Category = "Electronics",
                Qty = 8,
                Price = 15000
            }
        };
        }
    }
}
        
     
      
