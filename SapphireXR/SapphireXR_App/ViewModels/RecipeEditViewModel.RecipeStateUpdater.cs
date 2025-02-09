using OxyPlot;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeEditViewModel : ViewModelBase
    {
        internal class RecipeStateUpader: IDisposable
        {
            internal class ValveStateSubscriber : IObserver<bool>
            {
                internal ValveStateSubscriber(RecipeStateUpader recipeStateUpdater, string valveIDAssociated)
                {
                    stateUpdater = recipeStateUpdater;
                    valveID = valveIDAssociated;
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
                    if (stateUpdater.getValveState(valveID) != value)
                    {
                        stateUpdater.updateValve(valveID, value);
                    }
                }

                private string valveID;
                private RecipeStateUpader stateUpdater;
            }

            internal class ControlValueSubscriber : IObserver<float>
            {
                internal ControlValueSubscriber(RecipeStateUpader recipeStateUpdater, string flowControllerIDAssociated)
                {
                    stateUpdater = recipeStateUpdater;
                    flowControllerID = flowControllerIDAssociated;
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
                    if (stateUpdater.getControlValue(flowControllerID) != value)
                    {
                        stateUpdater.updateControlValue(flowControllerID, value);
                    }
                }

                private string flowControllerID;
                private RecipeStateUpader stateUpdater;
            }

            private void initializePublishSubscribe()
            {
                foreach ((string valveID, int index) in PLCService.ValveIDtoOutputSolValveIdx1)
                {
                    string topicName = "Valve.OnOff." + valveID + ".CurrentRecipeStep";

                    valveStatePublishers[valveID] = ObservableManager<bool>.Get(topicName);

                    ValveStateSubscriber valveStateSubscriber = new ValveStateSubscriber(this, valveID);
                    unsubscribers.Add(ObservableManager<bool>.Subscribe(topicName, valveStateSubscriber));
                    valveStateSubscribers.Add(valveStateSubscriber);
                }
                foreach ((string valveID, int index) in PLCService.ValveIDtoOutputSolValveIdx2)
                {
                    string topicName = "Valve.OnOff." + valveID + ".CurrentRecipeStep";

                    valveStatePublishers[valveID] = ObservableManager<bool>.Get(topicName);

                    ValveStateSubscriber valveStateSubscriber = new ValveStateSubscriber(this, valveID);
                    unsubscribers.Add(ObservableManager<bool>.Subscribe(topicName, valveStateSubscriber));
                    valveStateSubscribers.Add(valveStateSubscriber);
                }
                foreach ((string flowControllerID, int index) in PLCService.dIndexController)
                {
                    string topicName = "FlowControl." + flowControllerID + ".CurrentValue.CurrentRecipeStep";
                    flowValuePublishers[flowControllerID] = ObservableManager<float>.Get(topicName);

                    ControlValueSubscriber controlValueSubscriber = new ControlValueSubscriber(this, flowControllerID);
                    unsubscribers.Add(ObservableManager<float>.Subscribe(topicName, controlValueSubscriber));
                    controlStateSubscribers.Add(controlValueSubscriber);
                }
            }

            private static string GetFlowControllerID(string flowControlState)
            {
                string flowControllerID = "";
                switch (flowControlState)
                {
                    case "STemp":
                        flowControllerID = "Temperature";
                        break;

                    case "RPress":
                        flowControllerID = "Pressure";
                        break;

                    case "SRotation":
                        flowControllerID = "Rotation";
                        break;

                    default:
                        switch (flowControlState[0])
                        {
                            case 'M':
                                flowControllerID = "MFC" + flowControlState.Substring(1, flowControlState.Length - 1);
                                break;

                            case 'E':
                                flowControllerID = "EPC" + flowControlState.Substring(1, flowControlState.Length - 1);
                                break;
                        }
                        break;
                }

                return flowControllerID;
            }

            private void onRecipePropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != null)
                {
                    switch (args.PropertyName)
                    {
                        case "STemp":
                        case "RPress":
                        case "SRotation":
                            {
                                string flowControllerID = GetFlowControllerID(args.PropertyName);
                                propagateControlValue(flowControllerID, getControlValue(flowControllerID));
                            }
                            break;

                        default:
                            if (2 <= args.PropertyName.Length)
                            {
                                switch (args.PropertyName[0])
                                {
                                    case 'V':
                                        if (CheckDigit(args.PropertyName, 1) == true)
                                        {
                                            propagateValveState(args.PropertyName, getValveState(args.PropertyName));
                                        }
                                        break;

                                    case 'E':
                                    case 'M':
                                        {
                                            if (CheckDigit(args.PropertyName, 1) == true)
                                            {
                                                string flowControllerID = GetFlowControllerID(args.PropertyName);
                                                propagateControlValue(flowControllerID, getControlValue(flowControllerID));
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                    }

                }
            }

            public void setSelectedRecipeStep(Recipe recipe)
            {
                if(currentSelected != null)
                {
                    currentSelected.PropertyChanged -= onRecipePropertyChanged;
                }
                currentSelected = recipe;
                currentSelected.PropertyChanged += onRecipePropertyChanged;
                if(publishSubscribeInitialized == false)
                {
                    initializePublishSubscribe();
                    publishSubscribeInitialized = true;
                }
                propageSelectedRecipeStep(recipe);
            }

            private void propagateValveState(string valveID, bool isOpen)
            {
                valveStatePublishers[valveID].Issue(isOpen);
            }

            private void propagateControlValue(string flowControllerID, float value)
            {
                if (flowControllerID != string.Empty)
                {
                    flowValuePublishers[flowControllerID].Issue(value);
                }
            }

            static private bool CheckDigit(string str, int startIndex)
            {
                for (int letter = startIndex; letter < str.Length; ++letter)
                {
                    if (char.IsNumber(str[letter]) == false)
                    {
                        return false;
                    }
                }
                return true;
            }

            private void propageSelectedRecipeStep(Recipe value)
            {
                valveStatePublishers["V01"].Issue(value.V01);
                valveStatePublishers["V02"].Issue(value.V02);
                valveStatePublishers["V03"].Issue(value.V03);
                valveStatePublishers["V04"].Issue(value.V04);
                valveStatePublishers["V05"].Issue(value.V05);
                valveStatePublishers["V07"].Issue(value.V07);
                valveStatePublishers["V08"].Issue(value.V08);
                valveStatePublishers["V10"].Issue(value.V10);
                valveStatePublishers["V11"].Issue(value.V11);
                valveStatePublishers["V13"].Issue(value.V13);
                valveStatePublishers["V14"].Issue(value.V14);
                valveStatePublishers["V16"].Issue(value.V16);
                valveStatePublishers["V17"].Issue(value.V17);
                valveStatePublishers["V19"].Issue(value.V19);
                valveStatePublishers["V20"].Issue(value.V20);
                valveStatePublishers["V22"].Issue(value.V22);
                valveStatePublishers["V23"].Issue(value.V23);
                valveStatePublishers["V24"].Issue(value.V24);
                valveStatePublishers["V25"].Issue(value.V25);
                valveStatePublishers["V26"].Issue(value.V26);
                valveStatePublishers["V27"].Issue(value.V27);
                valveStatePublishers["V28"].Issue(value.V28);
                valveStatePublishers["V29"].Issue(value.V29);
                valveStatePublishers["V30"].Issue(value.V30);
                valveStatePublishers["V31"].Issue(value.V31);
                valveStatePublishers["V32"].Issue(value.V32);

                flowValuePublishers["MFC01"].Issue(value.M01);
                flowValuePublishers["MFC02"].Issue(value.M02);
                flowValuePublishers["MFC03"].Issue(value.M03);
                flowValuePublishers["MFC04"].Issue(value.M04);
                flowValuePublishers["MFC05"].Issue(value.M05);
                flowValuePublishers["MFC06"].Issue(value.M06);
                flowValuePublishers["MFC07"].Issue(value.M07);
                flowValuePublishers["MFC08"].Issue(value.M08);
                flowValuePublishers["MFC09"].Issue(value.M09);
                flowValuePublishers["MFC10"].Issue(value.M10);
                flowValuePublishers["MFC11"].Issue(value.M11);
                flowValuePublishers["MFC12"].Issue(value.M12);
                flowValuePublishers["MFC13"].Issue(value.M13);
                flowValuePublishers["MFC14"].Issue(value.M14);
                flowValuePublishers["MFC15"].Issue(value.M15);
                flowValuePublishers["MFC16"].Issue(value.M16);
                flowValuePublishers["MFC17"].Issue(value.M17);
                flowValuePublishers["MFC18"].Issue(value.M18);
                flowValuePublishers["MFC19"].Issue(value.M19);
                flowValuePublishers["EPC01"].Issue(value.E01);
                flowValuePublishers["EPC02"].Issue(value.E02);
                flowValuePublishers["EPC03"].Issue(value.E03);
                flowValuePublishers["EPC04"].Issue(value.E04);
                flowValuePublishers["EPC05"].Issue(value.E05);
                flowValuePublishers["EPC06"].Issue(value.E06);
                flowValuePublishers["EPC07"].Issue(value.E07);
                flowValuePublishers["Temperature"].Issue(value.STemp);
                flowValuePublishers["Pressure"].Issue(value.RPress);
                flowValuePublishers["Rotation"].Issue(value.SRotation);
            }

            public void clean()
            {
                ObservableManager<bool>.Get("Reset.CurrentRecipeStep").Issue(true);
                dispose();
            }

            private void dispose()
            {
                foreach(var unsubscriber in unsubscribers)
                {
                    unsubscriber.Dispose();
                }
                unsubscribers.Clear();
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        dispose();
                    }

                    disposedValue = true;
                }
            }

            void IDisposable.Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            private void updateValve(string valveID, bool isOpen)
            {
                if(currentSelected == null)
                {
                    return;
                }
                switch(valveID)
                {
                    case "V01":
                        currentSelected!.V01 = isOpen;
                        break;

                    case "V02":
                        currentSelected!.V02 = isOpen;
                        break;

                    case "V03":
                        currentSelected!.V03 = isOpen;
                        break;

                    case "V04":
                        currentSelected!.V04 = isOpen;
                        break;

                    case "V05":
                        currentSelected!.V05 = isOpen;
                        break;

                    case "V07":
                        currentSelected!.V07 = isOpen;
                        break;

                    case "V08":
                        currentSelected!.V08 = isOpen;
                        break;

                    case "V10":
                        currentSelected!.V10 = isOpen;
                        break;

                    case "V11":
                        currentSelected!.V11 = isOpen;
                        break;

                    case "V13":
                        currentSelected!.V13 = isOpen;
                        break;

                    case "V14":
                        currentSelected!.V14 = isOpen;
                        break;

                    case "V16":
                        currentSelected!.V16 = isOpen;
                        break;

                    case "V17":
                        currentSelected!.V17 = isOpen;
                        break;

                    case "V19":
                        currentSelected!.V19 = isOpen;
                        break;

                    case "V20":
                        currentSelected!.V20 = isOpen;
                        break;

                    case "V22":
                        currentSelected!.V22 = isOpen;
                        break;

                    case "V23":
                        currentSelected!.V23 = isOpen;
                        break;

                    case "V24":
                        currentSelected!.V24 = isOpen;
                        break;

                    case "V25":
                        currentSelected!.V25 = isOpen;
                        break;

                    case "V26":
                        currentSelected!.V26 = isOpen;
                        break;

                    case "V27":
                        currentSelected!.V27 = isOpen;
                        break;

                    case "V28":
                        currentSelected!.V28 = isOpen;
                        break;

                    case "V29":
                        currentSelected!.V29 = isOpen;
                        break;

                    case "V30":
                        currentSelected!.V30 = isOpen;
                        break;

                    case "V31":
                        currentSelected!.V31 = isOpen;
                        break;

                    case "V32":
                        currentSelected!.V32 = isOpen;
                        break;
                }
            }

            private bool getValveState(string valveID)
            {
                if (currentSelected == null)
                {
                    throw new Exception("RecipeStateUpdater: currentSelected is null in getValveState()");
                }
                switch (valveID)
                {
                    case "V01":
                        return currentSelected!.V01;

                    case "V02":
                        return currentSelected!.V02;

                    case "V03":
                        return currentSelected!.V03;

                    case "V04":
                        return currentSelected!.V04;

                    case "V05":
                        return currentSelected!.V05;

                    case "V07":
                        return currentSelected!.V07;

                    case "V08":
                        return currentSelected!.V08;

                    case "V10":
                        return currentSelected!.V10;

                    case "V11":
                        return currentSelected!.V11;

                    case "V13":
                        return currentSelected!.V13;

                    case "V14":
                        return currentSelected!.V14;

                    case "V16":
                        return currentSelected!.V16;

                    case "V17":
                        return currentSelected!.V17;

                    case "V19":
                        return currentSelected!.V19;

                    case "V20":
                        return currentSelected!.V20;

                    case "V22":
                        return currentSelected!.V22;

                    case "V23":
                        return currentSelected!.V23;

                    case "V24":
                        return currentSelected!.V24;

                    case "V25":
                        return currentSelected!.V25;

                    case "V26":
                        return currentSelected!.V26;

                    case "V27":
                        return currentSelected!.V27;

                    case "V28":
                        return currentSelected!.V28;

                    case "V29":
                        return currentSelected!.V29;

                    case "V30":
                        return currentSelected!.V30;

                    case "V31":
                        return currentSelected!.V31;

                    case "V32":
                        return currentSelected!.V32;

                    default:
                        throw new Exception("RecipeStateUpdater: " + valveID + " is invalid valve name in getValveState()");
                }
            }

            private void updateControlValue(string flowControllerID, float value)
            {
                if (currentSelected == null)
                {
                    return;
                }

                switch (flowControllerID)
                {
                    case "MFC01":
                        currentSelected.M01 = value;
                        break;

                    case "MFC02":
                        currentSelected.M02 = value;
                        break;

                    case "MFC03":
                        currentSelected.M03 = value;
                        break;

                    case "MFC04":
                        currentSelected.M04 = value;
                        break;

                    case "MFC05":
                        currentSelected.M05 = value;
                        break;

                    case "MFC06":
                        currentSelected.M06 = value;
                        break;

                    case "MFC07":
                        currentSelected.M07 = value;
                        break;

                    case "MFC08":
                        currentSelected.M08 = value;
                        break;

                    case "MFC09":
                        currentSelected.M09 = value;
                        break;

                    case "MFC10":
                        currentSelected.M10 = value;
                        break;

                    case "MFC11":
                        currentSelected.M11 = value;
                        break;

                    case "MFC12":
                        currentSelected.M12 = value;
                        break;

                    case "MFC13":
                        currentSelected.M13 = value;
                        break;

                    case "MFC14":
                        currentSelected.M14 = value;
                        break;

                    case "MFC15":
                        currentSelected.M15 = value;
                        break;

                    case "MFC16":
                        currentSelected.M16 = value;
                        break;

                    case "MFC17":
                        currentSelected.M17 = value;
                        break;

                    case "MFC18":
                        currentSelected.M18 = value;
                        break;

                    case "MFC19":
                        currentSelected.M19 = value;
                        break;

                    case "EPC01":
                        currentSelected.E01 = value;
                        break;

                    case "EPC02":
                        currentSelected.E02 = value;
                        break;

                    case "EPC03":
                        currentSelected.E03 = value;
                        break;

                    case "EPC04":
                        currentSelected.E04 = value;
                        break;

                    case "EPC05":
                        currentSelected.E05 = value;
                        break;

                    case "EPC06":
                        currentSelected.E06 = value;
                        break;

                    case "EPC07":
                        currentSelected.E07 = value;
                        break;

                    case "Temperature":
                        currentSelected.STemp = (short)value;
                        break;

                    case "Pressure":
                        currentSelected.RPress = (short)value;
                        break;

                    case "Rotation":
                        currentSelected.SRotation = (short)value;
                        break;

                    default:
                        throw new Exception("RecipeStateUpdater: currentSelected is null in updateControlValue()");
                }
            }

            private float getControlValue(string flowControllerID)
            {
                if (currentSelected == null)
                {
                    throw new Exception("RecipeStateUpdater: currentSelected is null in getControlValue()");
                }

                switch (flowControllerID)
                {
                    case "MFC01":
                        return currentSelected.M01;

                    case "MFC02":
                        return currentSelected.M02;

                    case "MFC03":
                        return currentSelected.M03;

                    case "MFC04":
                        return currentSelected.M04;

                    case "MFC05":
                        return currentSelected.M05;

                    case "MFC06":
                        return currentSelected.M06;

                    case "MFC07":
                        return currentSelected.M07;

                    case "MFC08":
                        return currentSelected.M08;

                    case "MFC09":
                        return currentSelected.M09;

                    case "MFC10":
                        return currentSelected.M10;

                    case "MFC11":
                        return currentSelected.M11;

                    case "MFC12":
                        return currentSelected.M12;

                    case "MFC13":
                        return currentSelected.M13;

                    case "MFC14":
                        return currentSelected.M14;

                    case "MFC15":
                        return currentSelected.M15;

                    case "MFC16":
                        return currentSelected.M16;

                    case "MFC17":
                        return currentSelected.M17;

                    case "MFC18":
                        return currentSelected.M18;

                    case "MFC19":
                        return currentSelected.M19;

                    case "EPC01":
                        return currentSelected.E01;

                    case "EPC02":
                        return currentSelected.E02;

                    case "EPC03":
                        return currentSelected.E03;

                    case "EPC04":
                        return currentSelected.E04;

                    case "EPC05":
                        return currentSelected.E05;

                    case "EPC06":
                        return currentSelected.E06;

                    case "EPC07":
                        return currentSelected.E07;

                    case "Temperature":
                        return currentSelected.STemp;
                        

                    case "Pressure":
                        return currentSelected.RPress;
                        

                    case "Rotation":
                        return currentSelected.SRotation;
                        

                    default:
                        throw new Exception("RecipeStateUpdater: return currentSelected is null in getControlValue()");
                }
            }

            private Dictionary<string, ObservableManager<bool>.DataIssuer> valveStatePublishers = new Dictionary<string, ObservableManager<bool>.DataIssuer>();
            private Dictionary<string, ObservableManager<float>.DataIssuer> flowValuePublishers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
            private IList<IObserver<bool>> valveStateSubscribers = new List<IObserver<bool>>();
            private IList<IObserver<float>> controlStateSubscribers = new List<IObserver<float>>();
            private IList<IDisposable> unsubscribers = new List<IDisposable>();

            public Recipe? currentSelected = null;
            private bool disposedValue = false;
            private bool publishSubscribeInitialized = false;
        }
    }
}
