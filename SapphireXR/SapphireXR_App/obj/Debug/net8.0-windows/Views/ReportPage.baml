   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  5      E'clr-namespace:SapphireXR_App.ViewModelsSapphireXR_App.ViewModels  5
      A%clr-namespace:SapphireXR_App.ControlsSapphireXR_App.Controls  5      %    SapphireXR_App.Views.ReportPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:SapphireXR_App.Views     5      R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   5      _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3b+http://schemas.microsoft.com/xaml/behaviors  5	      0vm'clr-namespace:SapphireXR_App.ViewModels  5
      3control%clr-namespace:SapphireXR_App.Controls  5        N� Title$  
ReportPage��5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��300 �6   �� 5      $��* �6   .+�5   
   � $��0��6   
�������$��Right=�66   $	��300��6R    � RowDefinitions 5      �� 5      $	��240 �6    �� 5      $��* �6    .+�5      �� $��0��6    � 	Resources 5       �� 
TargetType �� Property �� Value �� 	Resources��%F  '��       �� 5      " ��6   .�5      ��  � �� �	#FF0078D767   �� 5        � ��
 �����67   �� 5!       � ��$ 70��62   �� 5"       � ��$ 5,5,5,2q�63    5#      ���� 5$      '��       " ��6$   .�5%   "   ��  �@��CornerRadius$ 6��6A   5&      5'      5(      5)      .+�5*      �� $��30��6   .+�5+      ��  �� Orientation$ 
Horizontal=�6!   $��Center=�6:   .+�5,      �� -   5        	 ��Name$	 	PlotChart��5,   !   $
��Plot��64   5-      �� 5.      -   5        $	 PlotSave��5.   !   $
��Save��63   5/      50      51      �� 52      .+�53      �� $��14(�6    $��	10,20,0,0q�6.   
���   �$��Recipe File��6T   �� 54      $ 
Horizontal=�6!   $��Center=�6:   .+�55      �� -   5        
 ��Name$
 PlotRecipeFileOpen_1��55   "   $	��200��6@   $��25��6L   $��Bottom=�6X   .��6s    J� InputBindings 56      d� 57   "    d� MouseAction$ LeftDoubleClicke�6]    �� Command 6/   �� *PlotRecipeFileOpenCommand+6]   .��58   5   59      �� 5:      -   5        $	 PlotRecipeFileClose_1��5:   !   $��Remove��6@   $��Center=�6Q   ��5;   
   �� *PlotRecipeFileRemoveCommand+5<      5=      �� 5>      $ 
Horizontal=�6!   $��Center=�6:   .+�5?      �� -   5        $
 PlotRecipeFileOpen_2��5?   "   $	��200��6@   $��25��6L   $��Bottom=�6X   .��6s    5@      d� 5A   "   $ LeftDoubleClicke�6]    6/   �� *PlotRecipeFileOpenCommand+6]   .��5B   5   5C      �� 5D      -   5        $	 PlotRecipeFileClose_2��5D   !   $��Remove��6@   $��Center=�6Q   ��5E   
   �� *PlotRecipeFileRemoveCommand+5F      5G      �� 5H      $��14(�6    $��	10,20,0,0q�6.   
���   �$��Log File��6T   �� 5I      $ 
Horizontal=�6!   $��Center=�6:   .+�5J      �� -   5        $
 PlotLogFileOpen_1��5J   "   $	��200��6=   $��25��6I   $��Bottom=�6U   ��5K      ��  �� Path$ logFilepath1� �� UpdateSourceTrigger$ PropertyChanged=�.��6S    5L      d� 5M   "   $ LeftDoubleClicke�6[    6/   �� *PlotLogFileOpen1Command+6[   .��5N   5   5O      �� 5P      -   5        $	 PlotLogFileClose_1��5P   !   $��Remove��6=   $��Center=�6N   ��5Q   
   �� *PlotLogFileRemove1Command+5R      5S      �� 5T      $ 
Horizontal=�6!   $��Center=�6:   .+�5U      �� -	   5        $
 PlotLogFileOpen_2��5U   "   $	��200��6=   $��25��6I   $��Bottom=�6U   ��5V      �� $ logFilepath2�$ PropertyChanged=�.��6T    5W      d� 5X   "   $ LeftDoubleClicke�6[    6/   �� *PlotLogFileOpen2Command+6[   .��5Y   5   5Z      �� 5[      -
   5        $	 PlotLogFileClose_2��5[   !   $��Remove��6=   $��Center=�6N   ��5\   
   �� *PlotLogFileRemove2Command+5]      5^      �� 5_      $ 
Horizontal=�6!   $��Center=�6:   .+�5`      �� -   5        $	 btnPlotData1��5`   1   $��Data1��6!   $��Center=�6G   $��50��6i   ��5a   
   �� *btnPlotData1Command+�� 5b      -   5        $	 btnPlotData2��5b   1   $��Data2��6!   $��Center=�6G   $��50��6i   ��5c   
   �� *btnPlotData2Command+�� 5d      -   5        $	 btnPlotData3��5d   1   $��Data3��6!   $��Center=�6G   $��50��6i   ��5e   
   �� *btnPlotData3Command+�� 5f      -   5        $	 btnPlotData4��5f   1   $��Data4��6!   $��Center=�6G   $��50��6i   ��5g   
   �� *btnPlotData4Command+5h      5j      5k      � 5l      $��1��6   .+�5m      �� .+�5n      �� $ 
Horizontal=�6!   $��Center=�6:    5o      ��%j  '��       �� 5p      " ��6$   .�5q   "   ��  � �� �	#FF0078D76?   �� 5r   "    � ��
 �����6?   �� 5s   "    � ��$ 70��6:   �� 5t   "    � ��$ 20��6;   �� 5u   "    � ��$ 5,5,5,2q�6;    5v   "   ���� 5w   &   '��       " ��6,   .�5x   *   ��  �@��CornerRadius$ 6��6I   5y   '   5z   #   5{      5|      .+�5}      �� -   5        $	 btnPlotDevice��5}   !   $��Plot Device��68   $��70��6N   $��5q�6Y   ��5~   
   �� *btnPlotDeviceCommand+�� 5      $��14(�6$   $��10q�62   
���   �$
��Time��6Q   �� 5�      -   5        $
 PlotTime��5�   "   $��70��64   $��25��6?   5�      &   System.Windows.Controls.DataGrid  5�      -   5          Name$ PlotSelectedDevice��5�         AutoGenerateColumns$ False��6;   $��5q�6W      CanUserAddRows$ False��5�   
    �� AlternationCount$ 2��6!      AlternatingRowBackground
 �������5�   
   �� *ReportCompareData+ 5�      =  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�   '        '  r     �� 5�      "  6$   .�5�   "   ��  � ��$ Center=�6H   �� 5�   "    � ��
 ���� 6?   5�      �� 5�      "  6$   .�5�   "   ��  � ��
 �   �6@   �� 5�   "    � ��$ 1q�6D   5�      5�         Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 61      Header$ 	TimeStamp��6;   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6N   �� *	TimeStamp+ �� StringFormat$ {0:hh:mm:ss}��  5�      $ * 61   $ Data1��6;    6J   �� *Data1+$ 
{0:0.0000}��  5�      $ * 61   $ Data2��6;    6J   �� *Data2+$ 
{0:0.0000}��  5�      $ * 61   $ Data3��6;    6J   �� *Data3+$ 
{0:0.0000}��  5�      $ * 61   $ Data4��6;    6J   �� *Data4+$ 
{0:0.0000}��5�      5�      5�      5�      � 5�   
   $��1��6   
����·�.+�5�        OxyPlot.Wpf.PlotView    OxyPlot.Wpf.PlotViewBase   Model 6   �� *ReportComparePlot+5�      5�      5�      5�      