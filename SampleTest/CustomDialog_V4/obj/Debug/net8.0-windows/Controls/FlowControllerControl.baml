   M S B A M L   `   `   `  ����N  JCustomDialogSample1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:CustomDialogSample1.ControlsCustomDialogSample1.Controls  5      8    2CustomDialogSample1.Controls.FlowControllerControl   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:CustomDialogSample1.Controls     5      -   5          � 	Resources  5
       �� MergedDictionaries �� Source �� 
TargetType   BaseFontStyle �� BasedOn �� Property �� Value  MetroTextBoxStyle�� 5   
    5      �� 5      $ ../Themes/Generic.xaml?�6%   %  '��       7   '�� u     7  �� 5      " ��6   8   .�5      ��  � ��$ Center=�6=   �� 5       � ��$ 15(�6-   5      �� 5      " ��6   8   .�5      ��  � ��$ 15(�6-   �� 5       � ��$ Center=�6=   5      5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      �� 5      $��40 �6   .+�5    
   �� $��0��6   $	��5 5q�68   ��6   �� *Prompt+68   � 5"   
   $��1��6    5#      �� 5$      �� 5%      �� 5&       � ColumnDefinitions 5(      �� 5)      �� 5*      �� 5+      �� 5,      .+�5.      �� $��0��6   $��0��6!   .��61   Target Value6?   �� 5/      -   5        	 ��Name$	 txtInput��5/   3   $��0��6   $��1��6#   $��5q�6E   ��6P   �� *
Response+�� 51      $��1��6   $��0��6!   .��61   Ramp Time(s)6?   �� 52      -   5        $	 	txtInput1��52   3   $��1��6   $��1��6#   $��5q�6F   ��6Q   �� *
Response+�� 54      $��2��6   $��0��6!   .��61   	Deviation6<   �� 55      -   5        
 ��Name$
 	txtInput2��55   5   $��2��6   $��1��6%   $��5q�6H   ��6S   �� *
Response+�� 57      $��0��6   $��2��6!   .��61   Current Value6@   �� 58      -   5        $	 	txtInput3��58   3   $��0��6   $��3��6#   $��5q�6F   ��6Q   �� *
Response+�� 5:      $��1��6   $��2��6!   .��61   Control Value6@   �� 5;      -   5        $	 	txtInput4��5;   3   $��1��6   $��3��6#   $��5q�6F   ��6Q   �� *
Response+�� 5=      $��2��6   $��2��6!   .��61   
Max. Value6=   �� 5>      -   5        $	 	txtInput5��5>   3   $��2��6   $��3��6#   $��5q�6F   ��6Q   �� *
Response+5@      � 5B   
   $��2��6    5C      �� 5D      �� 5E      .+�5G      �� $��2��6   $��확인��5H      $��15(�5I      $��5q�5J       �� 	IsDefault$
 True��5K       � Cursor$
 Hand��5L        MetroBlueButtonStyle#��[ 5M      ��5N      �� *	OkCommand+��5O      ��  �� RelativeSource ��  �� AncestorType
 Window�� 5P      $��2��6   $��1��6"   $��취소��5Q      $��15(�5R      $��5q�5S       �� IsCancel$
 True��5T      $
 Hand��5U        MetroOrangeButtonStyle#��[ 5V      5W      5X      5Y      