   M S B A M L   `   `   `  ���� I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  E'clr-namespace:SapphireXR_App.ViewModelsSapphireXR_App.ViewModels  *    $SapphireXR_App.Views.BottomDashboard   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   0local"clr-namespace:SapphireXR_App.Views   2vm'clr-namespace:SapphireXR_App.ViewModels     N� Title$  BottomDashboard��.��� .+� � -     �Name$ frBottom��� ���#3e4a4b � ColumnDefinitions �� $	��800 ��� $�� * � � RowDefinitions �� $��20 ��� .+�� $��0��
�������$��0��.+���  �� Orientation$ 
Horizontal=� � 	Resources  �� 
TargetType �� Property �� Value	 �� 	Resources��%�   '��       �� " ��.���  � ��
 �   ���  � ��$ 76����  � ��$ 3,0,3,0q���  � ��$ Center=���  � ��$ Center=�	 ���� '��       " ��.���  �@��CornerRadius$ 8��.+��� $	��150��$��14(�$��SIGNAL PLOT��$��20,0,0,0q��� -   
 ��Name$
 BtnRecipeLive��$��
RecipeLive��
����������� *BtnRecipeLiveVisibleCommand+�� -   $
 BtnRecipeProgress��$��RecipeProgress��
����������� *!BtnRecipeProgressVisibleCommand+�� -   $
 BtnPlotAnalog��$��Analog��
��� ���$��	100,0,0,0q����� *BtnPlotAnalogCommand+�� -   $
 BtnPlotTemperatue��$��Temperature��
���  ������ *BtnPlotTemperatureCommand+�� -   $
 BtnPlotPessure��$��Pressure��
��� � ����� *BtnPlotPressureCommand+�� -   $
 BtnPlotTotalFlow��$��	TotalFlow��
����·����� *BtnPlotTotalFlowCommand+� $��0��
�������$��1�� �� $	��57* ��� $��7* ��� $	��29* ��� $	��13* ��� $	��54* �.+��� -    ��Name$ tabChart��$��5��.@�� .+�  OxyPlot.Wpf.PlotView  -	   $��
RecipeLive��y��� *btnRecipeLiveVisible+  OxyPlot.Wpf.PlotViewBase   Model �� *RecipeLivePlot+  -
   $��RecipeProgress��y��� *btnRecipeProgressVisible+ �� *RecipeProgressPlot+� $��1��$��0��
�������$��2,0,2,0q�.+��� $ 
Horizontal=� ��%�   '��       �� " ��.���  � ��
 �������  � ��$ 76����  � ��$ 3,0,3,0q���  � ��$ Center=���  � ��$ Center=�	 ���� '��       " ��.���  �@��CornerRadius$ 8��.+��� $	��150��
���   �$��14(�$��	EVENT LOG��$��20,0,0,0q��� -   $
 FilterAlarm��$��Alarm��
��� E���� -   $
 FilterWarning��$��Waring��
��� ����� -   $
 FilterInformation��$��Information��
���2�2��� -   $
 FilterClear��$��Clear��
�������� $��1��$��1�� &   System.Windows.Controls.DataGrid=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell �� Triggers P� Property P� Value��%�  '        '  m     '      �� "  .���   �@ HorizontalGridLinesBrush
 �  ����  �@ VerticalGridLinesBrush
 ��  ��� "  .���  � ��$ Center=���  � ��
 ��  ���  � ��$ 1q���  � ��
 � �����  �@ SeparatorVisibility$ Visible=��� "   P�  �@ 
IsSelected$
 True��. ���  � ��
 �������  � ��
 �**��.+�  -     Name$ dataGrid��   AutoGenerateColumns$
 True��   SelectionMode$ Extended=�   CanUserAddRows$ False�� �� AlternationCount$ 2��   AlternatingRowBackground
 ��������� *	EventData+ ��%    