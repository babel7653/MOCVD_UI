   M S B A M L   `   `   `  ���� I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  E'clr-namespace:SapphireXR_App.ViewModelsSapphireXR_App.ViewModels  A%clr-namespace:SapphireXR_APP.ControlsSapphireXR_APP.Controls  %    SapphireXR_App.Views.ReportPage   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3b+http://schemas.microsoft.com/xaml/behaviors  R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   0local"clr-namespace:SapphireXR_App.Views   2vm'clr-namespace:SapphireXR_App.ViewModels   5control%clr-namespace:SapphireXR_APP.Controls     N� Title$  Report�� � DataContext /   )SapphireXR_App.ViewModels.ReportViewModel  .���  � ColumnDefinitions �� $	��300 ��� $��* �.+�� $��0��
�������$��Right=�$	��300�� � RowDefinitions �� $	��240 ��� $��* �.+��� $��0�� � 	Resources  �� 
TargetType �� Property �� Value �� 	Resources��%�   '��       �� " ��.���  � �� �	#FF0078D7��  � ��
 �������  � ��$ 70����  � ��$ 5,5,5,2q� ���� '��       " ��.���  �@��CornerRadius$ 6��.+��� $��30��.+��� 	 �� Orientation$	 
Horizontal=�$��Center=�.+��� -   
 ��Name$
 	PlotChart��$
��Plot���� -   $
 PlotSave��$
��Save���� .+��� $��14(�$��	10,20,0,0q�
���   �$��Recipe File���� $	 
Horizontal=�$��Center=�.+��� -    ��Name$ PlotRecipeFileOpen_1��$	��200��$��25��$��Bottom=�.�� J� InputBindings d�  d� MouseAction$ LeftDoubleClicke� �� Command �� *PlotRecipeFileOpenCommand+.���� -   $
 PlotRecipeFileClose_1��$��Remove��$��Center=����� *PlotRecipeFileRemoveCommand+�� $	 
Horizontal=�$��Center=�.+��� -   $ PlotRecipeFileOpen_2��$	��200��$��25��$��Bottom=�.�� d� $ LeftDoubleClicke� �� *PlotRecipeFileOpenCommand+.���� -   $
 PlotRecipeFileClose_2��$��Remove��$��Center=����� *PlotRecipeFileRemoveCommand+�� $��14(�$��	10,20,0,0q�
���   �$��Log File���� $	 
Horizontal=�$��Center=�.+��� -   $ PlotLogFileOpen_1��$	��200��$��25��$��Bottom=�����  �� Path$ logFilepath1� �� UpdateSourceTrigger$ PropertyChanged=�.�� d� $ LeftDoubleClicke� �� *PlotLogFileOpen1Command+.���� -   $
 PlotLogFileClose_1��$��Remove��$��Center=����� *PlotLogFileRemove1Command+�� $	 
Horizontal=�$��Center=�.+��� -	   $ PlotLogFileOpen_2��$	��200��$��25��$��Bottom=����� $ logFilepath2�$ PropertyChanged=�.�� d� $ LeftDoubleClicke� �� *PlotLogFileOpen2Command+.���� -
   $
 PlotLogFileClose_2��$��Remove��$��Center=����� *PlotLogFileRemove2Command+�� $	 
Horizontal=�$��Center=�.+��� -   $
 btnPlotData1��$��Data1��$��Center=�$��50������ *btnPlotData1Command+�� -   $
 btnPlotData2��$��Data2��$��Center=�$��50������ *btnPlotData2Command+�� -   $
 btnPlotData3��$��Data3��$��Center=�$��50������ *btnPlotData3Command+�� -   $
 btnPlotData4��$��Data4��$��Center=�$��50������ *btnPlotData4Command+� $��1��.+��� .+��� $	 
Horizontal=�$��Center=� ��%�   '��       �� " ��.���  � �� �	#FF0078D7��  � ��
 �������  � ��$ 70����  � ��$ 20����  � ��$ 5,5,5,2q� ���� '��       " ��.���  �@��CornerRadius$ 6��.+��� -   $
 btnPlotDevice��$��Plot Device��$��70��$��5q����� *btnPlotDeviceCommand+�� $��14(�$��10q�
���   �$
��Time���� -   $ PlotTime��$��70��$��25��*   $SapphireXR_APP.Controls.DeviceSelect  &   System.Windows.Controls.DataGrid  -     Name$ PlotSelectedDevice��   AutoGenerateColumns$ False��$��5q�   CanUserAddRows$ False�� �� AlternationCount$ 2��   AlternatingRowBackground
 ��������� *ReportCompareData+ =  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�   '        '  ?     �� "  .���  � ��$ Center=���  � ��
 ���� �� "  .���  � ��
 �   ���  � ��$ 1q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ *    Header$ 	TimeStamp��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *	TimeStamp+ �� StringFormat$ {0:hh:mm:ss}��  $ * $ Data1�� �� *Data1+$ 
{0:0.0000}��  $ * $ Data2�� �� *Data2+$ 
{0:0.0000}��  $ * $ Data3�� �� *Data3+$ 
{0:0.0000}��  $ * $ Data4�� �� *Data4+$ 
{0:0.0000}��� $��1��
����·�.+�	  OxyPlot.Wpf.PlotView	  
  OxyPlot.Wpf.PlotViewBase 
  Model �� *ReportComparePlot+