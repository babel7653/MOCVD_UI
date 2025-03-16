namespace SapphireXR_App.ViewModels
{
    public class MonitoringMeterViewModel : PresentValueMonitorViewModel
    {
        protected override void updatePresentValue(float value)
        {
            PresentValue = ((int)value).ToString();
        }
    }
}
