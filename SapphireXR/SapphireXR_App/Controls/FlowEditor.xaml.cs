using OxyPlot;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SapphireXR_App.Controls
{
    public partial class FlowEditor : UserControl
    {   
        public FlowEditor()
        {
            InitializeComponent();
        }
        public string ControllerID
        {
            get { return (string)GetValue(ControllerIDProperty); }
            set { SetValue(ControllerIDProperty, value); }
        }
        public static readonly DependencyProperty ControllerIDProperty =
           DependencyProperty.Register("ControllerID", typeof(string), typeof(FlowEditor), new PropertyMetadata(default));

        public float EditValue
        {
            get { return (float)GetValue(EditValueProperty); }
            set { SetValue(EditValueProperty, value); }
        }

        public static readonly DependencyProperty EditValueProperty =
            DependencyProperty.Register("EditValue", typeof(float), typeof(FlowEditor), new PropertyMetadata(default));

        public string ContentLabel
        {
            get { return (string)GetValue(contentLabelProperty); }
            set { SetValue(contentLabelProperty, value); }
        }
        static uint ContentLabelCount = 0;
        public readonly DependencyProperty contentLabelProperty = 
            DependencyProperty.Register("ContentLabelProperty" + (ContentLabelCount++), typeof(string), typeof(FlowEditor), new PropertyMetadata(default));
    }
}
