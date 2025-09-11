using System.Globalization;
using System.Windows.Data;

namespace SapphireXR_App.ValueConverter
{
    internal class NullableNumericValueConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        object? IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? targetValue = value as string;
            if (targetValue != null)
            {
                if (targetValue == string.Empty)
                {
                    return null;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
