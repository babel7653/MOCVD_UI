   M S B A M L   `   `   `  ����N  JCustomDialogSample1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:CustomDialogSample1.ViewsCustomDialogSample1.Views  5   	   .    (CustomDialogSample1.Views.OkCancelDialog   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:CustomDialogSample1.Views     5   	     � WindowStartupLocation$  CenterOwner=�5   	    � Title$ OkCancelDialog��5	   	   $	��200��6    $	��500��6-    � 	Resources 5
       �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� RadiusX	 �� RadiusY
 �� Triggers P� Property P� Value �� 
TargetName��%.  '��       '�� �    �� 5   
   " ��6   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5       � ��$ Segoe UI)�6+   �� 5       � ��$ 15(�6)   �� 5       � ��$
 8 16q�6(   �� 5       � ��$ 0q�60   �� 5       � ��$ 10q�6'   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ PART_Background��6(   #��z��6A   $ 2��6e   $	 2��6q   �� 5      $��Center=�6/   $��Center=�6L   5      
 5      P� 5       �@J�IsMouseOver$
 True��6=   . �5   "   �� $ PART_Background��6)    � �� �#64B5F66V   5      P� 5       � ��$
 True��6;   . �5    "   �� $ PART_Background��6)    � �� �#1976D26V   5!      P� 5"       � {�$ False��6;   . �5#   "   ��  �@J�Opacity$	 0.7Q�6<   5$      5&      5(      5)      �� 5*   
   " ��6   .�5+      ��  � ��$ 15(�6)   �� 5,       � ��$ 15q�6'   5-      5.      .��5/      �  � RowDefinitions 50   
   �� 51      �� 52      $��50 �6   .+�54   
   �� -   5         ��Name$ tbMessanger��54      � 55   
   $��1��6    � ColumnDefinitions 56      �� 57      �� 58      .+�5:      �� -   5         ��Name$ btnOk��5:      $��0��5;      $��확인��5<       �� 	IsDefault$
 True��5=      5>      �� 5?      $��1��6   $��취소��5@       �� IsCancel$
 True��5A      5B      5C      5D      