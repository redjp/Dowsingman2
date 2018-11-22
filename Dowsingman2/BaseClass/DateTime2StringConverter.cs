using System;
using System.Globalization;
using System.Windows.Data;

namespace Dowsingman2.BaseClass
{
    [ValueConversion(typeof(DateTime?), typeof(string))]
    public class DateTime2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = value as DateTime?;
            return dateTime?.ToString("yyyy/M/d H:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
