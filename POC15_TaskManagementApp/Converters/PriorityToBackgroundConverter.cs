using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace POC15__TaskManagementApp.Converters
{
    public class PriorityToBackgroundConverter : IValueConverter
    {
        
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                    if (value == null)
                        return Brushes.White;

                    switch (value.ToString())
                    {
                        case "High":
                            return Brushes.LightCoral;

                        case "Medium":
                            return Brushes.Moccasin;

                        case "Low":
                            return Brushes.LightGreen;

                        default:
                            return Brushes.White;
                    }
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                    throw new NotImplementedException();
                }
            }
        }
    


