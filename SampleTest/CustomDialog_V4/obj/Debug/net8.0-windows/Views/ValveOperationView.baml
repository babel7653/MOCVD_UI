   M S B A M L   `   `   `  ����N  JCustomDialogSample1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:CustomDialogSample1.ViewsCustomDialogSample1.Views  5   	   2    ,CustomDialogSample1.Views.ValveOperationView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:CustomDialogSample1.Views     5   	   -   5          �Name$  valve��5   	    � WindowStyle$
 None=�5   	   
������  � AllowsTransparency$
 True��5
   	    � SizeToContent$ WidthAndHeight=�5   	   $	��400��5   	   $	��150��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$ ValveOperationMessage��5   	   $	��150��6'   $	��400��64    � 	Resources 5       �� MergedDictionaries �� Source	 �� 
TargetType
 �� Property �� Value�� 5   
    5      �� 5      $ ../Themes/Generic.xaml?�6%   %  '��       '�� l     �� 5      "	 ��6   .�5      �� 
 � ��$ 15(�6-   �� 5      
 � ��$ 10q�6+   5      �� 5      "	 ��6   .�5      �� 
 � ��
 �����6/   �� 5      
 � ��$ 5q�6+   �� 5      
 � ��$
 Bold"�6/   5      5       .��5"      ��  �� CornerRadius$ 5��6   
�������._�5#   
   � 
������  � RowDefinitions 5$      �� 5%      $��40 �6    �� 5&      �� 5'      $��40 �6    .+�5*      �� $ 5 5 0 0��6   
����iA�._�5+      �� $��0��6   ��5,      �� *Title+5-      �� 5/      $��1��6   $��40��6%   ��61   �� *	Message+� 51      $��2��6   $��Center=�6     � ColumnDefinitions 52      �� 53      $
��Auto �6'   �� 54      $
��Auto �6'   .+�57      �� $��0��6   $��15(�58      $��5q�59      $	��120��5:         MetroBlueButtonStyle#��[  5;       �� 	IsDefault$
 True��5<      ��5=      �� *OkText+��5>      �� *	OkCommand+��5?      ��  �� ElementName$ valve���� 5@      $��1��6   $��15(�5A      $��5q�5B      $	��120��5C        MetroOrangeButtonStyle#��[ 5D       �� IsCancel$
 True��5E      ��5F      �� *
CancelText+��5G      �� *CanelCommand+��5H      �� $ valve��5I      5J      5K      5L      