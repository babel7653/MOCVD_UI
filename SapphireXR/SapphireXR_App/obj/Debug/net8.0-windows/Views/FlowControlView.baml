   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  5   	   A%clr-namespace:SapphireXR_App.ControlsSapphireXR_App.Controls  5      *    $SapphireXR_App.Views.FlowControlView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:SapphireXR_App.Views     5   	   9control%clr-namespace:SapphireXR_App.Controls     5      -   5          �Name$  FlowControlDialog��5   	    � WindowStyle$
 None=�5	   	    � AllowsTransparency$
 True��5
   	   
������  � WindowStartupLocation$ CenterScreen=�5   	    � SizeToContent$ WidthAndHeight=�5   	   $	��400��5   	   $	��150��5   	    � Title$ FlowControl��5   	   $	��250��6   $	��400��6*    � 	Resources 5       �� MergedDictionaries �� Source	 �� 
TargetType   BaseFontStyle
 �� BasedOn �� Property �� Value�� 5   
    5      �� 5      $ ../Themes/Generic.xaml?�6%   %  '��       7   '�� u     7   �� 5      "	 ��6   8
   .�5      ��  � ��$ 13(�6-   �� 5       � ��$ Center=�6=   5      �� 5      "	 ��6   8
   .�5      ��  � ��$ 13(�6-   �� 5        � ��$ Center=�6=   5!      5"      .��5%      ��  �� CornerRadius$ 5��6   
�������._�5&   
   � 
������  � RowDefinitions 5'      �� 5(      $��40 �6    �� 5)      .+�5,      �� $ 5 5 0 0��6   
����iA�._�5-      �� $��0��6   ��5.      �� *Title+5/      � 51      $��1��6   $��10q�6     52      �� 53      $��40 �6$   �� 54      �� 55      $��40 �6$   .+�57      �� $��0��6   $	��5 5q�6@   ��6%   �� *Prompt+6@   � 59      $��1��6    5:      �� 5;      �� 5<      �� 5=       � ColumnDefinitions 5?      �� 5@      �� 5A      �� 5B      �� 5C      .+�5E      �� $��0��6   $��0��6)   .��69   Target Value6G   �� 5F      -   5         ��Name$ txtInput��5F   ;   $��0��6   $��1��6+   $��5q�6M   ��6X   �� *
Response+�� 5H      $��1��6   $��0��6)   .��69   Ramp Time(s)6G   �� 5I      -   5        $ 	txtInput1��5I   ;   $��1��6   $��1��6+   $��5q�6N   ��6Y   �� *
Response+�� 5K      $��2��6   $��0��6)   .��69   	Deviation6D   �� 5L      -   5        $ 	txtInput2��5L   ;   $��2��6   $��1��6+   $��5q�6N   ��6Y   �� *
Response+�� 5N      $��0��6   $��2��6)   .��69   Current Value6H   �� 5O      -   5        $ 	txtInput3��5O   ;   $��0��6   $��3��6+   $��5q�6N   ��6Y   �� *
Response+�� 5Q      $��1��6   $��2��6)   .��69   Control Value6H   �� 5R      -   5        $ 	txtInput4��5R   ;   $��1��6   $��3��6+   $��5q�6N   ��6Y   �� *
Response+�� 5T      $��2��6   $��2��6)   .��69   
Max. Value6E   �� 5U      -   5        $ 	txtInput5��5U   ;   $��2��6   $��3��6+   $��5q�6N   ��6Y   �� *
Response+5W      � 5Y      $��2��6    5Z      �� 5[      �� 5\      .+�5^      �� $��2��6   $��확인��5_      $��15(�5`      $��5q�5a       �� 	IsDefault$
 True��5b       � Cursor$
 Hand��5c        MetroBlueButtonStyle#��[ 5d      ��5e      �� *	OkCommand+��5f      ��  �� RelativeSource ��  �� AncestorType
 Window�� 5g      $��2��6   $��1��6*   $��취소��5h      $��15(�5i      $��5q�5j       �� IsCancel$
 True��5k      $
 Hand��5l        MetroOrangeButtonStyle#��[ 5m      5n      5o      5s      5t      5v      