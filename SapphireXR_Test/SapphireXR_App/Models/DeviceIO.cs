﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.Models
{
  public class Device : ObservableObject
  {
    public string? ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
  }
  public class GasAIO : Device
  {
    public int MaxValue { get; set; }
    public int MinSignal { get; set; }
    public float CurrentValue { get; set; }
    public float TargetValue { get; set; }
    //public int ControlValue { get; set; }
    //public int RampTime { get; set; }
    public bool AlarmSet { get; set; }
    public bool WarningSet { get; set; }
    public int UserState1 { get; set; }
    public int UserState2 { get; set; }
    public int UserState3 { get; set; }
    public int UserState4 { get; set; }

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
  public class Sensor : Device
  {
    public int MaxValue { get; set; }
    public int MinSignal { get; set; }
    public int MaxSignal { get; set; }
    public bool AlarmSet { get; set; }
    public bool WarningSet { get; set; }
  }
  public class SwitchDI : Device
  {
    public bool IsOn { get; set; }
    public bool AlarmSet { get; set; }
    public bool WarningSet { get; set; }

  }
  public class SwitchDO : Device
  {
    public bool IsOn { get; set; }
  }
  public class InterLockA
  {
    public bool IsEnable { get; set; }
    public string Treshold { get; set; }
  }
  public class UserState
  {
    public int StartState { get; set; }
    public int AlarmState { get; set; }
    public int EndState { get; set; }
  }
  public enum EUserState : int
  {
    UserState1 = 0,
    UserState2 = 1,
    UserState3 = 2,
    UserState4 = 3
  }
  public class COMIO : Device
  {
    public TypeOfCom typeOfCom { get; set; }
  }
  public enum TypeOfCom : int
  {
    DeviceNet = 0,
    RS232 = 1,
    RS485 = 2
  }
}
