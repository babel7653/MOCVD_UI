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
        private class FlowControllerLabelUpdater : IObserver<(string, string)>
        {
            public FlowControllerLabelUpdater(FlowControllerViewModelBase vm)
            {
                flowControllerViewModelBase = vm;
            }

            void IObserver<(string, string)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, string)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, string)>.OnNext((string, string) value)
            {
                if(flowControllerViewModelBase.ControllerID == value.Item1)
                {
                    flowControllerViewModelBase.Name = value.Item2;
                }
            }

            private FlowControllerViewModelBase flowControllerViewModelBase;
        }

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
            if (param == null)
            {                 
                throw new Exception("FlowControllerViewModelBase onLoaded parameter is null");
            }
            object[]? args = param as object[];
            if(args == null || args.Length < 2)
            {
                throw new Exception("FlowControllerViewModelBase onLoaded parameter is not valid");
            }
            string? type = args[0] as string;
            if (type == null || type == string.Empty)
            {
                throw new Exception("Type property of FlowController not set");
            }
            string? controllerID = args[1] as string;
            if (controllerID == null || controllerID == string.Empty)
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
