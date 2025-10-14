using System.Globalization;
using System.Windows.Data;
using SapphireXR_App.Models;

namespace SapphireXR_App.ValueConverter
{
    internal class MOMaterialToMOSourceFormualaControlConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MOSourceModel.MOMaterial material = (MOSourceModel.MOMaterial)value;
            switch (material)
            {
                case MOSourceModel.MOMaterial.Liquid:
                    return new Controls.MOSource1();

                case MOSourceModel.MOMaterial.Solid:
                    return new Controls.MOSource4();

                default:
                    throw new InvalidCastException("Invalid MOMaterial Enum Value");
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
