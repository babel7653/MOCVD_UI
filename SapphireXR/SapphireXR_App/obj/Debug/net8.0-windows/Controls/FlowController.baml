   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:SapphireXR_App.ControlsSapphireXR_App.Controls  5      ,    &SapphireXR_App.Controls.FlowController   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:SapphireXR_App.Controls     5      -   5          >�Name$  flowController��5       � 	Resources 5
         ControllerStyle �� 
TargetType �� Property �� Value �� 
TargetType � RowDefinitions  MFCDisplayColor1 �� CornerRadius �� Orientation	 ��Name  MFCDisplayColor2
 �� Triggers P� Property P� Value��%�  &	        7  7  �� 5   
   " ��6(   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �  5      �� 5   "   $��14 �60   �� 5   "   .+�5      �� 
�������$��1,1,1,0q�6>   8��  $ 3,3,0,0��6�   ._�5   "   �� $ 
Horizontal=�6-   .+�5   &   �� $	 flowSet��6,   $��16��6>   $��50��6J   $��12(�6U   $��Right=�6c   $��Center=�6�   $��0q�6�   $��1234.5��5   ,   
��� � �6=   A� 5   &   $��10��6.   $��10��69   
��� � �$��Right=�6Q   $��5,0,0,0q�6n   5   #   5      �� 5      $��1��6%   
�������$��1,0,1,1q�6K   8�� $ 0,0,3,3��6�   ._�5   "   �� $ 
Horizontal=�6/   .+�5   &   �� $	 flowCurrent��6,   $��28��6A   $��68��6M   $��20(�6X   $��Right=�6f   $��Center=�5      $��0q�68   $��1234.5��6D   5   #   5       5!      
 5"      P� 5#       � {�$
 True��6;   5$      P� 5%       � {�$ False��6;   5&      5(      5*      5+      5,      .��5-      �� #��[  6   5.      