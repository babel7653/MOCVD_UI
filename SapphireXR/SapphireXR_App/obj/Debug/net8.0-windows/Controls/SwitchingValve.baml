   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:SapphireXR_App.ControlsSapphireXR_App.Controls  5      ,    &SapphireXR_App.Controls.SwitchingValve   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:SapphireXR_App.Controls     5      -   5          >�Name$  switchingValve��5       � 	Resources 5
         SwitchingValveStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius � ColumnDefinitions A�Name	 �� Triggers
 P� Property P� Value �� 
TargetName��%�  &	        �� 5   
   " ��6,   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �� ���#475569$ 3��66   ._�5      � $��24��6#   $��48��6/    5   "   �� 5   &   �� 5   &   .+�5   "   A� $ on��6*   $��0��66   $��16��6F   $��16��6Q   A� 5   "   $	 off��6*   $��1��67   $��16��6G   $��16��6R   5      5      	 5      P� 5      
 � {�$
 True��6;   . �5   "   �� $ on��6)    � ��
 �����6I   �� 5   "   $	 off��6)    � ��
 � E��6J   5      P� 5      
 � {�$ False��6;   . �5   "   �� $ on��6)    � ��
 � � �6I   �� 5    "   $	 off��6)    � ��
 �����6J   5!      5#      5%      5&      5'      .��5(   
   �� #��[  6   5)      