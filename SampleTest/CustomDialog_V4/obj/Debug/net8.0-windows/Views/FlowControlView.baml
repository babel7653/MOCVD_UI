   M S B A M L   `   `   `  ����N  JCustomDialogSample1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:CustomDialogSample1.ViewsCustomDialogSample1.Views  5   	   K*clr-namespace:CustomDialogSample1.ControlsCustomDialogSample1.Controls  5   	   O,clr-namespace:CustomDialogSample1.ViewModelsCustomDialogSample1.ViewModels  5   	   /    )CustomDialogSample1.Views.FlowControlView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:CustomDialogSample1.Views     5   	   ?controls*clr-namespace:CustomDialogSample1.Controls     5   	   ;vm,clr-namespace:CustomDialogSample1.ViewModels     5   	     � WindowStyle$
  None=�5
   	    � AllowsTransparency$
 True��5   	   
������  � WindowStartupLocation$ CenterScreen=�5   	    � SizeToContent$ WidthAndHeight=�5   	   $	��400��5   	   $	��150��5   	    � Title$ FlowControl��5   	   $	��300��6   $	��400��6*    � 	Resources 5       �� DataType9   3CustomDialogSample1.ViewModels.FlowControlViewModel8   2CustomDialogSample1.Controls.FlowControllerControl �� 
TargetType �� Property	 �� Value��%  (��       5      *, +)'�� .     �� 6
   "  6   .R�5        5      �� 5   
   " ��6   .�5      ��  � ��
	 �����6+   �� 5       � ��$	 5q�6'   �� 5       � ��$	 15(�6)   �� 5       � ��$
	 Bold"�6+   5      5      .��5      �� 
 �� CornerRadius$
 5��6   
�������._�5    
   � 
������  � RowDefinitions 5!      �� 5"      $��40 �6    �� 5#      .+�5&      �� $
 5 5 0 0��6   
����iA�._�5'      �� $��0��6   ��5(      �� *Title+5)      �� 5*      $��1��6   ��6*   �� 5+      5,      5-      