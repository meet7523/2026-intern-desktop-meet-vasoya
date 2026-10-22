using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Converters
{

    public class BooleanToRequirementConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            bool isValid = value is bool && (bool)value;

            string requirement =
                parameter == null
                    ? string.Empty
                    : parameter.ToString();

            if (isValid)
            {
                return "✓  " + requirement;
            }

            return "○  " + requirement;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

