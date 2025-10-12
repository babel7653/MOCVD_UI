using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;

namespace SapphireXR_App.Models
{
    internal partial class MOSourceModel: ObservableObject, IObserver<float>, IObserver<bool>
    {
        static readonly Dictionary<string, (string, string)> ConnectedInput = new Dictionary<string, (string, string)>() { { "Source1", ("MFC08", "V07") }, { "Source2", ("MFC09", "V10") }, { "Source3", ("MFC10", "V13") }, 
            { "Source4", ("MFC11", "V16") }, { "Source5", ("MFC12", "V19") }, { "Source6", ("MFC15", "V22") } };
        internal enum MOMaterial { Liquid = 0, Solid }

        internal static MOMaterial GetMaterialBySourceName(string sourceName)
        {
            switch (sourceName)
            {
                case "Source1":
                case "Source2":
                case "Source3":
                case "Source4":
                case "Source5":
                    return MOMaterial.Liquid;

                case "Source6":
                    return MOMaterial.Solid;

                default:
                    throw new ArgumentException("Invalid source name");
            }
        }

        internal MOSourceModel(string sourceName)
        {
            MFC = ConnectedInput[sourceName].Item1;
            Valve = ConnectedInput[sourceName].Item2;

            update = PLCService.ReadValveState(Valve);

            PropertyChanged += (sender, args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(BubblerTemp):
                        if (BubblerTemp != null)
                        {
                            T = BubblerTemp + AbsoluteTemp;
                        }
                        else
                        {
                            T = null;
                        }
                        break;

                    case nameof(AValue):
                    case nameof(BValue):
                    case nameof(T):
                        if (AValue != null && BValue != null && T != null)
                        {
                            try
                            {
                                switch (GetMaterialBySourceName(sourceName))
                                {
                                    case MOMaterial.Liquid:
                                        PartialPressure = (float)Math.Pow(10.0, AValue.Value - (BValue.Value / T.Value));
                                        break;
                                    case MOMaterial.Solid:
                                        PartialPressure = (float)Math.Pow(10.0, AValue.Value - (2.18 * Math.Log(Math.E, T.Value)) - (BValue.Value / T.Value));
                                        break;
                                }
                            }
                            catch
                            {
                                PartialPressure = null;
                            }
                        }
                        else
                        {
                            PartialPressure = null;
                        }
                        break;

                    case nameof(PartialPressure):
                    case nameof(BubblePressure):
                    case nameof(BubblerConst):
                    case nameof(MolarWeight):
                    case nameof(QMFC):
                        if (PartialPressure != null && BubblePressure != null && BubblerConst != null && MolarWeight != null && QMFC != null)
                        {
                            try
                            {
                               weightDelta = QMFC * PartialPressure.Value / (BubblePressure - PartialPressure) / (22400 * 60) * MolarWeight.Value * BubblerConst.Value;
                            }
                            catch
                            {
                                weightDelta = null;
                            }
                        }
                        else
                        {
                            weightDelta = null;
                        }
                        break;

                    case nameof(InitialWeight):
                        if(InitialWeight != null)
                        {
                            UsedWeight = 0.0f;
                        }
                        else
                        {
                            UsedWeight = null;
                        }
                        break;

                    case nameof(UsedWeight):
                        if (UsedWeight != null && InitialWeight != null)
                        {
                            float remainWeightUpdate = InitialWeight.Value - UsedWeight.Value;
                            RemainWeight = Math.Max(0.0f, remainWeightUpdate);
                        }
                        else
                        {
                            RemainWeight = null;
                        }
                        break;

                    case nameof(RemainWeight):
                        if (0.0f < RemainWeight)
                        {
                            Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ =>
                            {
                                if(UsedWeight == null || weightDelta == null || update == false)
                                {
                                    return;
                                }

                                UsedWeight += weightDelta;

                            }, TaskScheduler.FromCurrentSynchronizationContext());
                        }
                        break;
                }
            };

            ObservableManager<float>.Subscribe("FlowControl." + MFC + ".CurrentValue", this);
            ObservableManager<bool>.Subscribe("Valve.OnOff." + Valve + ".CurrentPLCState", this);
        }

        void IObserver<float>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<float>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<float>.OnNext(float value)
        {
            QMFC = value;
        }

        void IObserver<bool>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<bool>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<bool>.OnNext(bool value)
        {
            update = value;
        }

        [ObservableProperty]
        private float? aValue= null;

        [ObservableProperty]
        private float? bValue= null;

        [ObservableProperty]
        private float? t= null;

        [ObservableProperty]
        private float? bubblerTemp= null;

        [ObservableProperty]
        private float? partialPressure= null;

        [ObservableProperty]
        private float? bubblePressure= null;

        [ObservableProperty]
        private float? bubblerConst= null;

        [ObservableProperty]
        private float? molarWeight= null;

        [ObservableProperty]
        private float? initialWeight= null;

        [ObservableProperty]
        private float? usedWeight= null;

        [ObservableProperty]
        private float? remainWeight= null;

        [ObservableProperty]
        private float? qMFC = null;

        private string MFC { get; set; }
        private string Valve { get; set; }

        private float? weightDelta = null;
        private bool update;

        private static readonly float AbsoluteTemp = 273.15f;
    }
}
