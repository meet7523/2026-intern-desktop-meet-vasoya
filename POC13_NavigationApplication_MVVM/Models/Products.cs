using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13_NavigationApplication_MVVM.Models
{
    public class Products
    {
        public string ProductName { get; set; }
        public string Category { get; set; }

        public int Qty { get; set; }
        public decimal Price {  get; set; }
    }
}
