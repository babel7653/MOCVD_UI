using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace SapphireXR_App.ValueConverter
{
    internal class StringToMOSourceFormulaControlConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sourceName = value as string ?? string.Empty;
            switch(sourceName)
            {                 
                case "Source1":
                case "Source2":
                case "Source3":
                case "Source4":
                case "Source5":
                    return new Controls.MOSource1();
                case "Source6":
                    return new Controls.MOSource4();
                default:
                    return "";
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
