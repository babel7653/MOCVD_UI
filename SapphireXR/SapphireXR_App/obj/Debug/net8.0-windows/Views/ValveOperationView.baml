   M S B A M L   `   `   `  ����I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  5   	   -    'SapphireXR_App.Views.ValveOperationView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:SapphireXR_App.Views     5   	   -   5          �Name$  valveDialog��5   	    � WindowStyle$
 None=�5   	   
������  � AllowsTransparency$
 True��5
   	    � SizeToContent$ WidthAndHeight=�5   	   $	��400��5   	   $	��150��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$ ValveOperationPopup��5   	   $	��150��6%   $	��400��62    � 	Resources 5       �� MergedDictionaries �� Source	 �� 
TargetType
 �� Property �� Value�� 5   
    5      �� 5      $ ../Themes/Generic.xaml?�6%   %  '��       '�� l     �� 5      "	 ��6   .�5      �� 
 � ��$ 15(�6-   �� 5      
 � ��$ 10q�6+   5      �� 5      "	 ��6   .�5      �� 
 � ��
 �����6/   �� 5      
 � ��$ 5q�6+   �� 5      
 � ��$
 Bold"�6/   5       5!      .��5$      ��  �� CornerRadius$ 5��6   
�������._�5%   
   � 
������  � RowDefinitions 5&      �� 5'      $��40 �6    �� 5(      �� 5)      $��40 �6    .+�5,      �� $ 5 5 0 0��6   
����iA�._�5-      �� $��0��6   ��5.   
   �� *Title+5/      �� 51      $��1��6   $��40��6%   ��61   �� *	Message+� 53      $��2��6   $��Center=�6     � ColumnDefinitions 54      �� 55      $
��Auto �6'   �� 56      $
��Auto �6'   .+�59      �� $��0��6   $��15(�5:      $��5q�5;      $	��120��5<         MetroBlueButtonStyle#��[  5=       �� 	IsDefault$
 True��5>      $��확인��5?      ��5@      �� *	OkCommand+��5A      ��  �� ElementName$ valve���� 5B      $��1��6   $��15(�5C      $��5q�5D      $	��120��5E        MetroOrangeButtonStyle#��[ 5F       �� IsCancel$
 True��5G      $��취소��5H      ��5I      �� *CanelCommand+��5J      �� $ valve��5K      5L      5M      5N      