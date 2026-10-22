using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Converters
{
    public class PlaceholderVisibilityConverter : IMultiValueConverter
    {
        public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
        {
            string text = values[0] as string;
            bool isFocused = values[1] is bool focused && focused;

            if (string.IsNullOrEmpty(text) && !isFocused)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
