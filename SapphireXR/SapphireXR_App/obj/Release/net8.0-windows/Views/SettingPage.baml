   M S B A M L   `   `   `  ���� I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  &     SapphireXR_App.Views.SettingPage   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:SapphireXR_App.Views     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors    N� Title$  Setting��.��� .+���  �� TabStripPlacement$
 Left=� � 	Resources  �� 
TargetType �� Property �� Value � LayoutTransform �� Angle��%�   '��       �� " ��.���  � �� �� .R��� #��z�� �� $	 270Q���  � ��$ 20����  � ��$ 90����  � ��$ 3q�.@��� -   $��AlarmSetting��$��Alarm Setting����� $
��1290�� � RowDefinitions �� $��30 ��� $��* �.+�� $��0��
��� ���.+��� $��14(�$��Alarm / Warning Setting��$
��Bold"�
�������$��Center=�$��Center=�� $��1��.+��� 	 �� Orientation$	 
Horizontal=�.+��� $��Alarm Setting (Analog)��$	��430��
����İ�$��2q�$��2,10,2,0q�$��14(�.���  �� $��25 ��� $��25 ��� .+��� $��0��$	 
Horizontal=�.+��� $��12(�$��25��$	��120��$��Delay Time(s)���� -   
 ��Name$
 tbAlarmDelayTimeA��$��70��$��20������  �� Path$ dPreSet[AlarmDelayTimeA]��� -    ��Name$ btnAlarmSettingSave��$
��Save��$��70��$��Right=�$��10,0,0,0q����� *AlarmSettingSaveCommand+ ��%9   '��       �� " ��.���  �@��CornerRadius$ 6���� $��1��$	 
Horizontal=�.+��� $��12(�$��25��$	��120��$��Alarm Deviation(%)���� -   $
 tbAlarmDeviatin��$��70��$��20������ $ dPreSet[AlarmDeviation]��� $��12(�$��25��$	��120��$��10,0,5,0q�$��Warning Deviation(%)���� -   $
 tbWarningDeviation��$��70��$��20������ $ dPreSet[WarningDeviation]��� $��2��.+�&   System.Windows.Controls.DataGrid  -     Name$ dgAlarmSettingA��   AutoGenerateColumns$ False��$��5,0,5,0q�"   VerticalScrollBarVisibility$
 Auto=�   CanUserAddRows$ False������ *	lGasAIO+ =  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ ID��   CanUserSort$ False��   CanUserResize$ False��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� $ ID�  $ Device Name��$ False��$ False��$ *  �� $
 Name�4  .System.Windows.Controls.DataGridCheckBoxColumn  $ Alarm��$ 50 $ False�� �� $ AlarmSet�  $ Warning��$ 50 $ False�� �� $ 
WarningSet��� $��Alarm Setting (Digital)��$	��430��
����İ�$��2q�$��2,10,2,0q�$��14(�.���  �� $��25 ��� $��25 ��� .+��� $��0��$	 
Horizontal=�.+��� $��12(�$��25��$	��120��$��Delay Time(s)���� -   $
 tbAlarmDelayTimeD��$��70��$��20������ $ dPreSet[AlarmDelayTimeD]��� -   $ btnEventSettingSave��$
��Save��$��70��$��Right=�$��10,0,0,0q����� *AlarmSettingSaveCommand+ ��%9   '��       �� " ��.���  �@��CornerRadius$ 6���� $��2��.+�  -	   $ dgAlarmSettingD��$ False��$��5,0,5,0q�$
 Auto=�$ False������ *	lSwitchDI+ ��%I   '        �� "  .���  � ��$ Center=���  � ��
 ����    $ ID��$ False��$ False��$ 50  �� $ ID�  $ Device Name��$ False��$ False��$ *  �� $
 Name�  $ Alarm��$ False��$ False��$ 50  �� $ AlarmSet�  $ Warning��$ False��$ False��$ 50  �� $ 
WarningSet��� $��LogBook��$	��420��
����İ�$��2q�$��2,10,2,0q�$��14(�.���  �� $��25 ��� $��25 ��� .+��� $��0��  �� 	Resources��%V   '��       �� " �� ���� '��       " ��.���  �@��CornerRadius$ 4��.+��� -
   $ LogFileRead��$
��Read��$��70��$��25,0,0,0q��� $��1��$	 
Horizontal=�.+��� $��12(�$��25��$��70��$��Filter���� -   $ LogFilterAlarm��$��Alarm��$��50��$��25,0,0,0q�
���  ���� -   $ LogFilterWarning��$��Warning��$��50��$��25,0,0,0q�
��� ����� -   $ LogFilterInformation��$��Information��$��50��$��25,0,0,0q�
��� � ��� -   $ LogFilterClear��$��Clear��$��50��$��25,0,0,0q�
��������� $��2��.+�  -   $ LogbookTable��$ False��$��5,0,5,0q� ��%I   '        �� "  .���  � ��$ Center=���  � ��
 ����    $
 Type��$ False��$ False��$ 60  ��   $ Massage��$ False��$ False��$ *  ��   $
 Date��$ False��$ False��$	 100  �� *Date+ �� StringFormat$ d��  $
 Time��$ False��$ False��$	 100  �� *Time+�� -   $��UserStateSetting��$��Safety Setting�����  �� $��30 ��� .+�� $��0��
��� � �.+��� $��14(�$��Safety Setting��$
��Bold"�
�������$��Center=�$��Center=�� $��1�� �� $	��28* ��� $
��723* � � ColumnDefinitions �� $	��21* ��� $	��13* �.+��� $	 
Horizontal=�$��2��$��2��.+��� $��User State Setting��$	��640��
����İ�$��2q�$��2,10,2,0q�$��14(�.���� $��0��.+��� -   $ UserStateSave��$
��Save��$��70��$��5q�$��Right=����� *AlarmSettingSaveCommand+ ��%9   '��       �� " ��.���  �@��CornerRadius$ 6��� $	��670�� �� $	��50* ��� $
��Auto ��� $	��50* �.+��� $��0��.+�  -   $ dgUserSettingA��$ False��$��5,0,5,0q�$	��320��$
 Auto=�$ False������ *	lGasAIO+ ��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   $ 	Device Id��$ False��$ False��$ 60  �� $ ID� �� Mode$ TwoWay=�  $ Device Name��$ False��$ False��$ *  �� $
 Name�  $ State 1��$ False��$ False��$	 100  �� $ 
UserState1�  $ State 2��$ False��$ False��$	 100  �� $ 
UserState2�  $ State 3��$ False��$ False��$	 100  �� $ 
UserState3�  $ State 4��$ False��$ False��$	 100  �� $ 
UserState4��� $��1��$��15���� $��2��.+�  -   $ dgUserSettingD��$ False��$��5,0,5,0q�$	��320��$
 Auto=�$ False������ *lGasDO+ ��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   $ 	Device Id��$ False��$ False��$ 60  �� $ ID�$ TwoWay=�  $ Device Name��$ False��$ False��$ *  �� $
 Name�  $ State 1��$ False��$ False��$	 100  �� $ 
UserState1�  $ State 2��$ False��$ False��$	 100  �� $ 
UserState2�  $ State 3��$ False��$ False��$	 100  �� $ 
UserState3�  $ State 4��$ False��$ False��$	 100  �� $ 
UserState4��� .+��� $��Interlock Setting��$	��640��
����İ�$��2q�$��2,10,2,0q�$��14(�.���  �� ��  �� $��35 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $��30 �.+��� $��0��$��0��$��2��$��Double O-Ring Release��$
��Bold"�$��Bottom=��� -   $ InterlockSave��$��0��$��1��$
��Save��$��70��$
��Left=����� *AlarmSettingSaveCommand+ ��%9   '��       �� " ��.���  �@��CornerRadius$ 6���� -    ��Name$ cbRecipeNotRunning��$��1��$��0��$��Recipe Not Running��$��Center=�$��15,0,0,0q� d� 	IsChecked �� $# dInterLockD[RecipeNotRunning]��� -   $ cbReactorPressureLimit��$��2��$��0��$��Reactor Pressure(Torr) >=��$��Center=�$��15,0,0,0q� �� $. (dInterLockA[DORReactorPressure].IsEnable��� -   $
 tbReactorPressureLimit��$��2��$��1��$	��150��$
��Left=�$��2q����� $. (dInterLockA[DORReactorPressure].Treshold��� -   $ cbSusceptorTemperature��$��3��$��0��$��Susceptor Temperature��$��Center=�$��15,0,0,0q� �� $3 -dInterLockA[DORSusceptorTemperature].IsEnable��� -   $
 DORTemperatureSet��$��3��$��1��$	��150��$
��Left=�$��2q����� $3 -dInterLockA[DORSusceptorTemperature].Treshold��� $��4��$��0��$��2��$��Double O-Ring Lock��$
��Bold"�$��Bottom=��� $��5��$��0��$�� Vacuum Valve Close��$��Center=�$��15,0,0,0q� �� $# dInterLockD[VacuumValveClose]��� $��6��$��0��$��Pressure Limit��$��Center=�$��15,0,0,0q� �� $, &dInterLockA[DORPressureLimit].IsEnable��� -   $
 DORPressureLimit��$��6��$��1��$	��150��$
��Left=�$��2q����� $3 -dInterLockA[DORSusceptorTemperature].Treshold��� $��7��$��0��$�� Alarm Limit Value��$��Center=�$��15,0,0,0q� �� $&  dInterLockA[AlarmLimit].IsEnable��� -   $
 DORAlarmLimit��$��7��$��1��$	��150��$
��Left=�$��2q����� $&  dInterLockA[AlarmLimit].Treshold��� $��8��$��0��$��2��$��Auto Power ON��$
��Bold"�$��Bottom=��� $��9��$��0��$�� Induction Power Supply��$��Center=�$��15,0,0,0q� �� $' !dInterLockD[InductionPowerSupply]��� $��10��$��0��$�� Susceptor Rotation Motor��$��Center=�$��15,0,0,0q� �� $) #dInterLockD[SusceptorRotationMotor]��� $��11��$��0��$�� Reactor Cabinet Lamp��$��Center=�$��15,0,0,0q� �� $% dInterLockD[ReactorCabinetLamp]��� $��12��$��0��$�� Gas Control Cabinet Lamp��$��Center=�$��15,0,0,0q� �� $! dInterLockD[GasControlLamp]��� $��Preset State��
����İ�$��2q�$��2,10,2,0q�$��14(�.���  �� �� �� �� .+��� $��0��$	 
Horizontal=�$��2q�.+��� $	��200��$��System Start State���� -    ��Name$ comboSystemStart��$	��100�� �� 
IsReadOnly$
 True�� �� SelectedIndex �� $ userstate.StartState��� $��1��$	 
Horizontal=�$��2q�.+��� $	��200��$��Alarm State���� -   $��comboAlarmStart��$	��100��$
 True�� �� $ userstate.AlarmState��� $��2��$	 
Horizontal=�$��2q�.+��� $	��200��$��
Recipe End���� -   $��comboRecipeEnd��$	��100��$
 True�� �� $ userstate.EndState��� -   $��DeviceSetting��$��Device Setting�����  �� $��30 ��� .+�� $��0��
����  �.+��� $��14(�$��Device Setting��$
��Bold"�
�������$��Center=�$��Center=�� $��1��.+��� $	 
Horizontal=�.+��� $��Device Naming��$	��620��
����İ�$��2q�$��2,10,2,0q�$��14(�.���� .+��� -    $ DeviceNameSave��$
��Save��$��70��$��Right=�$��0,0,50,0q� ��%9   '��       �� " ��.���  �@��CornerRadius$ 6��� $	��670�� �� $	��50* ��� $
��Auto ��� $	��50* �.+��� .+�  -!   $ MaxValueSetting��$ False��$��5,0,5,0q� ��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   $ 	Device ID��$ False��$ False��$ 60  ��   $ Device Name��$ False��$ False��$ *  ��   $ 	Max Value��$ False��$ False��$	 120  ��   $ Ramping��$ False��$ False��$ 60  ��   $ Accuracy��$ False��$ False��$ 60  ��   $ Units��$ False��$ False��$ 60  �� �� $��1��$��Stretch=�$��Center=�
����İ�  �� ShowsPreview$
  True��$��3���� $��2��$	 
Horizontal=�.+��� .+�  -"   $ DigitalMappingSetting��$	��405��$ False��$��5,0,5,0q� ��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   $ 	Device ID��$ False��$ False��$ 60  ��   $ Device Name��$ False��$ False��$ *  ��   $ Sol. ID��$ False��$ False��$	 120  �� �� -#   ! ��Name$! SolenoidValveID��$	��190���� $��Component I/O List��$	��660��
����İ�$��2q�$��2,10,2,0q�$��14(�.���� .+�  -$   $ IOList��$ False��$��5,0,5,0q� ��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   $ ID��$ False��$ False��$ 60  ��   $
 Name��$ False��$ False��$ *  ��   $ 	Component��$ False��$ False��$	 120  ��   $
 Type��$ False��$ False��$ 60  ��   $ Signal��$ False��$ False��$ 60  ��   $ Terminal��$ False��$ False��$	 100  ��   $ Pin No.��$ False��$ False��$	 100  ��   $ 	Org.Value��$ False��$ False��$ 60  ��   $ 
Cal. Value��$ False��$ False��$ 60  �� �� -%   $��IO_List��$��I/O List�����  �� $��30 ��� .+�� $��0��
��� ���.+��� $��14(�$��I/O List��$
��Bold"�
���   �$��Center=�$��Center=�� $��1��.+�  -&   $��IO_ListTable��$��5q�