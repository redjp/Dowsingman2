using System;
using System.Globalization;
using System.Windows.Data;

namespace Dowsingman2.BaseClass
{
    [ValueConversion(typeof(int), typeof(string))]
    public class Listener2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value < 0 ? null : value + "人";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
