using System.Windows.Controls;
using SapphireXR_App.Models;

namespace SapphireXR_App.Controls
{
  /// <summary>
  /// SourceControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class SourceControl : UserControl
  {
    public SourceControl()
    {
      InitializeComponent();
    }

    private GasAIO _device;
    public GasAIO Device
    {
      set
      {
        _device = value;
        SetValue.Content = _device.TargetValue;
        CurValue.Content = _device.CurrentValue;
      }
      get
      {
        return _device;
      }
    }
  }
}
