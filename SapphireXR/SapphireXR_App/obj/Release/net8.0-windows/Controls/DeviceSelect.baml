   M S B A M L   `   `   `  ���� I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:SapphireXR_APP.ControlsSapphireXR_APP.Controls  E'clr-namespace:SapphireXR_App.ViewModelsSapphireXR_App.ViewModels  *    $SapphireXR_APP.Controls.DeviceSelect   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3b+http://schemas.microsoft.com/xaml/behaviors  1local%clr-namespace:SapphireXR_APP.Controls  0vm'clr-namespace:SapphireXR_App.ViewModels  .��� 
�������  � RowDefinitions  �� $��30 ��� $��* ��� $��30 �.+��� $��Plot Tag Select�� �� TextAlignment$ Center=�
����ح�$	��290��$��25��$��Center=�$��Center=�$��16(�$��5,5,5,0q��� $��1�� �� Orientation$ 
Horizontal=�.+��� -    ��Name$ LbxLeft��$	��100��$��5q� �� SelectedItem$ Selected�� �� SelectionMode$ Multiple=����� *LeftListPlotTag+ �� Mode$ OneWay=� �� NotifyOnSourceUpdated$
 True������ .R��� ���� *  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger  	   	EventName$	 SelectionChanged��*  $Microsoft.Xaml.Behaviors.TriggerBase
   Actions.
 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command �� *$"SelectedItemLeftListPlotTagCommand+   CommandParameter ��  �� ElementName$ LbxLeft�� �� Path$ SelectedItem��� $��80��$��Center=�.+��� -    ��Name$ CbSelectPlotTagPV��$��PV��$��2q�
������� d� 	IsChecked$
 TrueY����� *CbSelectPlotTagPvCommand+�� -   $ CbSelectPlotTagSV��$��SV��$��2q�
�������$
 TrueY����� *CbSelectPlotTagSvCommand+�� -   $ CbSelectPlotTagEtc��$	��Etc��$��2q�
�������$
 TrueY����� *CbSelectPlotTagEtcCommand+�� -    ��Name$ btnMoveRightPlotTag��$��>>��$��2q�
����������� *BtnMoveRightPlotTagCommand+�� -   $ btnMoveLeftPlotTag��$��<<��$��2q�
����������� *BtnMoveLeftPlotTagCommand+�� -   $ btnMoveRightAllPlotTag��$��All >>��$��2q�
����������� *BtnMoveRightAllPlotTagCommand+�� -   $ btnMoveLeftAllPlotTag��$��<< All��$��2q�
����������� *BtnMoveLeftAllPlotTagCommand+�� -	   $ LbxRight��$	��100��$��5q����� *RightListPlotTag+$ TwoWay=��� $��2��$��Select��
����ح�$	��290��$��25��$��Center=�$��Center=�$��5,0,5,5q�