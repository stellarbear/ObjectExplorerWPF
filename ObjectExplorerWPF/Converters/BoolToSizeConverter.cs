using System;
using System.Globalization;
using System.Windows.Data;

namespace ObjectExplorerWPF.Converters
{
    public class BoolToSizeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 32 : 24;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
