   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:SapphireXR_App.ControlsSapphireXR_App.Controls  5      )    #SapphireXR_App.Controls.BypassValve   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:SapphireXR_App.Controls     5      -   5          >�Name$  bypassValve��5       � 	Resources 5
         BlockValveStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius � RowDefinitions	 �� X1		 �� Y1	
 �� X2	 �� Y2 A�Name �� Orientation �� Triggers P� Property P� Value �� 
TargetName��%  &	        �� 5   
   " ��6(   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �� 
�������$ 3��68   ._�5      � $��40��6#   $��60��6/    5   "   �� 5   &   �� 5   &   .+�5   "   �� $ 10��6'   $	 11��6/   $
 50��67   $ 11��6?   
���� ��$��2��6X   �� 5   "   $ 10��6'   $	 0��6/   $
 10��66   $ 20��6>   
���� ��$��2��6W   �� 5   "   $ 50��6'   $	 0��6/   $
 50��66   $ 20��6>   
���� ��$��2��6W   A� 5   "   $ bypass��6*   $��0��6:   $��20��6G   $��20��6R   �� 5   "   $��1��6-   $ 
Horizontal=�6:   $��Center=�6S   .+�5   &   A� $ in��6.   $��20��6:   $��20��6E   $��0,0,10,0q�6Q   A� 5   &   $	 out��6.   $��20��6;   $��20��6F   $��10,0,0,0q�6R   5   #   5      5       5       P� 5!       � {�$ False��6;   . �5"   "   �� $ bypass��6)    � ��
 �����6M   �� 5#   "   $ in��6)    � ��
 �  ��6I   �� 5$   "   $	 out��6)    � ��
 �  ��6J   5%      P� 5&       � {�$
 True��6;   . �5'   "   �� $ bypass��6)    � ��
 � � �6M   �� 5(   "   $ in��6)    � ��
 �����6I   �� 5)   "   $	 out��6)    � ��
 �����6J   5*      5,      5.      5/      50      .��51      �� .+�52   
   �� $��Center=�6   #��[  6,   53      54      