using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bookmaker.Converters
{
    [ValueConversion(typeof(System.Enum), typeof(System.Array))]
    public class EnumToValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            return Enum.GetNames(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
