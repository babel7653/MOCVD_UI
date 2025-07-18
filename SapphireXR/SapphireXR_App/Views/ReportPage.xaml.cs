using SapphireXR_App.ViewModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SapphireXR_App.Views
{
    public partial class ReportPage : Page
    {
        public ReportPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(ReportViewModel));
        }
    }

    public class ChartModeListToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<ReportViewModel.Mode> modeList = (List<ReportViewModel.Mode>)value;
            return modeList.Select(mode =>
            {
                if (mode == ReportViewModel.Mode.DataValue)
                {
                    return "Data Value";
                }
                else
                {
                    return "Percentage";
                }
            });
           
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented for ChartModeListToStringConverter.");
        }
    }

    public class ChartModeToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                ReportViewModel.Mode mode = (ReportViewModel.Mode)value;
                if (mode == ReportViewModel.Mode.DataValue)
                {
                    return "Data Value";
                }
                else
                {
                    return "Percentage";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReportPage.xml에서 List<ReportViewModel.Mode>을 IEnumerable<string>으로 변환하는데 문제가 발생했습니다. 원인은 다음과 같습니다: " + ex.Message);
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string strValue = (string)value;

                if (strValue == "Data Value")
                {
                    return ReportViewModel.Mode.DataValue;
                }
                else
                {
                    return ReportViewModel.Mode.Percentage;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReportPage.xml에서 string을 ReportViewModel.Mode로 변환하는데 문제가 발생했습니다. 원인은 다음과 같습니다: " + ex.Message);
            }
        }
    }
}
