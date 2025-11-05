using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.Models
{
    public partial class Device : ObservableObject
    {
        public string? ID { get; set; }
        [ObservableProperty]
        private string? _name;
        public string? Description { get; set; }
    }
    public partial class WarningAlarmDevice: Device
    {
        [ObservableProperty]
        private bool _alarmSet;
        [ObservableProperty]
        private bool _warningSet;
    }

    public partial class AnalogDeviceIO : WarningAlarmDevice
    {
        [ObservableProperty]
        private int maxValue;
    }

    public class GasDO : Device
    {
        public bool IsOn { get; set; }
        public bool NormallyClose { get; set; }
        public bool UserState1 { get; set; }
        public bool UserState2 { get; set; }
        public bool UserState3 { get; set; }
        public bool UserState4 { get; set; }

    }

    public partial class SwitchDI : WarningAlarmDevice
    {
        public bool IsOn { get; set; }
    }
    public class SwitchDO : Device
    {
        public bool IsOn { get; set; }
    }
    public partial class InterLockA: ObservableObject
    {
        [ObservableProperty]
        private bool _isEnable;
        [ObservableProperty]
        private string _treshold = "";
    }
    public partial class InterLockD: ObservableObject
    {
        [ObservableProperty]
        private bool _isEnable;
    }
 
}
