using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Bases;
using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;

namespace SapphireXR_App.ViewModels.FlowController
{
    public abstract partial class FlowControllerViewModelBase : ViewModelBase, INotifyPropertyChanged
    {
        static FlowControllerViewModelBase()
        {
            MouseEnterColor = Application.Current.Resources.MergedDictionaries[0]["ValveOnMouseEnterColor"] as SolidColorBrush ?? MouseEnterColor;
        }

        private static readonly SolidColorBrush DefaultBorderBackground = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush DefaultMouseEnterColor = new SolidColorBrush(Color.FromRgb(0x9d, 0xbc, 0xe8));
        private static readonly SolidColorBrush DefaultControllerBorderBackground = new SolidColorBrush(Color.FromRgb(0xCC, 0xDF, 0xEF));

        public string Type { get; set; } = "";

        [ObservableProperty]
        public string controllerID = "";
        [ObservableProperty]
        private string _controlValue = "";
        [ObservableProperty]
        private bool? _isDeviationLimit;
        [ObservableProperty]
        public SolidColorBrush _borderBackground = DefaultBorderBackground;
        [ObservableProperty]
        private string? _name = "";

        public static SolidColorBrush MouseEnterColor
        {
            get; set;
        } = DefaultMouseEnterColor;
        public SolidColorBrush ControllerBorderBackground
        {
            get; set;
        } = DefaultControllerBorderBackground;

        protected virtual void onLoaded(object? param)
        {
            string type = (string)((object[])param!)[0];
            if (type == string.Empty)
            {
                throw new Exception("Type property of FlowController not set");
            }

            string controllerID = (string)((object[])param)[1];
            if (controllerID == string.Empty)
            {
                throw new Exception("ControllerID property of FlowController not set");
            }

            onLoaded(type, controllerID);
        }

        protected virtual void onLoaded(string type, string controllerID)
        {
            Type = type;
            ControllerID = controllerID;
            var getName = (Dictionary<string, string> renameMapping) =>
            {
                string? name = renameMapping.Where((KeyValuePair<string, string> keyValue) => keyValue.Value == ControllerID).Select((KeyValuePair<string, string> keyValue) => keyValue.Key).FirstOrDefault();
                if (name != null)
                {
                    Name = Util.GetFlowControllerName(name!);
                }
            };
            switch (Type)
            {
                case "MFC":
                    ControllerBorderBackground = Application.Current.Resources.MergedDictionaries[0]["MFCDisplayColor2"] as SolidColorBrush ?? ControllerBorderBackground;
                    getName(Util.RecipeFlowControlFieldToControllerID);
                    break;

                case "EPC":
                    ControllerBorderBackground = Application.Current.Resources.MergedDictionaries[0]["EPCDisplayColor1"] as SolidColorBrush ?? ControllerBorderBackground;
                    getName(Util.RecipeFlowControlFieldToControllerID);
                    break;

                case "Reactor":
                    ControllerBorderBackground = Application.Current.Resources.MergedDictionaries[0]["ReactorDisplayColor1"] as SolidColorBrush ?? ControllerBorderBackground;
                    getName(ReactorID);
                    break;
            }
            BorderBackground = ControllerBorderBackground;
        }

        public ICommand OnLoadedCommand => new RelayCommand<object?>(onLoaded);

        protected abstract void onClicked(object[]? args);

        public ICommand OnMouseEntered => new RelayCommand(() =>
        {
            BorderBackground = MouseEnterColor;
        });
        public ICommand OnMouseLeaved => new RelayCommand(() =>
        {
            BorderBackground = ControllerBorderBackground;
        });

        public ICommand OnClickedCommand => new RelayCommand<object[]?>(onClicked);

        private static readonly Dictionary<string, string> ReactorID = new () { { "R01", "Temperature" }, { "R02", "Pressure"  }, { "R03", "Rotation" } };
    }
}
