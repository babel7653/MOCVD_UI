   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:SapphireXR_App.ControlsSapphireXR_App.Controls  5      +    %SapphireXR_App.Controls.LeakTestValve   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:SapphireXR_App.Controls     5      -   5          >�Name$  leakTestValve��5       � 	Resources 5
         BlockValveStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius � ColumnDefinitions A�Name		 �� X1	
 �� Y1	 �� X2	 �� Y2 �� Triggers P� Property P� Value �� 
TargetName��%  &	        �� 5   
   " ��6(   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �� 
�������$ 3��68   ._�5      � $��30��6#   $��45��6/    5   "   �� 5   &   $��30 �67   �� 5   &   .+�5   "   A� $ on��6*   $��0��66   $��20��6F   $��20��6Q   �� 5   "   $��1��6'   $	 8��67   $
 2��6>   $ 8��6E   $ 28��6L   
���� ��$��2��6e   �� 5   "   $��1��6'   $	 -2��67   $
 15��6?   $ 8��6G   $ 15��6N   
���� ��$��2��6g   5      5       5      P� 5       � {�$
 True��6;   . �5   "   �� $ on��6)    � ��
 �����6I   5      P� 5       � {�$ False��6;   . �5   "   �� $ on��6)    � ��
 �  ��6I   5!      5#      5%      5&      5'      .��5(      �� .+�5)   
   �� $��Center=�6   #��[  6,   5*      5+      