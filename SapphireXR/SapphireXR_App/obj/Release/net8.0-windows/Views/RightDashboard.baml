   M S B A M L   `   `   `  ���� I  ESapphireXR_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SapphireXR_App.ViewsSapphireXR_App.Views  )    #SapphireXR_App.Views.RightDashboard   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:SapphireXR_App.Views       N� Title$  RightDashboard��.��� -    �Name$ gridMenuRight��$��2�� � RowDefinitions �� $��* ��� $	��240 �.+��� $��0��   SideBackgroundColor#���   � $��0��.+���  � 	Resources  �� 
TargetType �� Property  PrimaryFontColor �� Value��%h  '��       '�� �     'A� �     �� " ��.���  � ��# �  ��  � ��$ 
Pretendard)���  � ��$ 14(���  � ��$	 200����  � ��$ 20,2,0,2q��� " ��.���  � ��# �  ��  � ��$ 
Pretendard)���  � ��$ 14(��� " A�.���  � ��$ 10����  � ��$ 10����  � ��$ 10,0,0,0q���  � ��
 � �|�.+��� $	��200��$	��Top=�.+��� $��Center=�$��2q��� .�Current State�� 	 �� X1$ 5��	 �� Y1$ 0��		 �� X2$		 295��	
 �� Y2$
 0��
����  �$��2����  �� Orientation$ 
Horizontal=�.+��� -    ��Name$ GasState��$��14(�$��
Idle State��$	��100��$��10,1,0,1q��� -    ��Name$ GasCurrentValue��$	��100��$��10,1,0,1q�.��100A� -    A�Name$ GasCurrentState���� $ 
Horizontal=�.+��� -   $ ChamberState��$��14(�$��Chamber Open��$	��100��$��10,1,0,1q��� -   $ ChamberCurrentValue��$��14(�$	��100��$��10,1,0,1q�.��100A� -   $ ChamberCurrentState���� $ 
Horizontal=�.+��� -   $ 	LeakState��$��14(�$��	Leak Test��$	��100��$��10,1,0,1q��� -	   $ LeakCurrentValue��$��14(�$	��100��$��10,1,0,1q�.��Leak Test ModeA� -
   $ LeakCurrentState���� $ 
Horizontal=�.+��� -   $ SoftStopState��$��14(�$��	Soft STOP��$	��100��$��10,1,0,1q��� -   $ SoftStopValue��$��14(�$	��100��$��10,1,0,1q�.��50A� -   $ SoftStopCurrentState���� $	��230��$	��Top=�.+��� $��Center=�$��2q�$
��Bold"�$��Enable State���� $ 5��$ 0��$		 295��$
 0��
����  �$��2���� $ 
Horizontal=�.+��� $��Recipe Allowed��A� -   $ RecipeAllowedState���� $ 
Horizontal=�.+��� $��Control Allowed��A� -   $ ControlAllowedState���� $ 
Horizontal=�.+��� $��H2 Gas Supply��A� -   $ H2GasSupplyState��
���  ���� $ 
Horizontal=�.+��� $��Hydride Gas Supply��A� -   $ HydrideGasSupplyState��
���  ���� $ 
Horizontal=�.+��� $��Reactor Open Enable��A� -   $ ReactorOpenState���� $ 
Horizontal=�.+��� $��Reactor Open Timer��A� -   $ ReactorOpenTime���� $ 
Horizontal=�.+��� $��Heating Allowed��A� -   $ HeatingAllowedState���� $ 
Horizontal=�.+��� $��Double O-Ring��A� -   $ DORState���� $	��200��$	��Top=�.+��� 
����  �$��14(�$��Center=�$��2q��� .�Utility State�� $ 5��$ 0��$		 295��$
 0��
����  �$��2���� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��Pneumatic PressureA� -   $ PneumaticPressureState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��Cooling WaterA� -   $ CoolingWaterState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��Reactor Cabinet DoorA� -   $ ReactorDoorState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��Gas Cabinet DoorA� -   $ GasDoorState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��Control Cabinet DoorA� -   $ ControlDoorState��$��10��$��10��
��� �|�$��10,0,0,0q��� $	��Top=�.+��� 
����  �$��14(�$��Center=�$��2q��� .�Equipment State�� $ 5��$ 0��$		 295��$
 0��
����  �$��2���� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��AlarmA� -   $ 
AlarmState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��	WarningA� -   $ WarningState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��200��	SilenceA� -   $ SilenceState��$��10��$��10��
��� �|�$��10,0,0,0q��� $ 
Horizontal=�.+��� $��14(�$��20,2,2,2q�$	��170��User�� $	��100��.��Administartor�� $��1��  PrimaryBackgroundColor#���  
���   �$��1q�� $��1��   Analog Device Stroke��%D   &	       �� " ��.���  � ��
 �   ���  � ��$ 1�� � ColumnDefinitions �� $	��120 ���  �� $��25 ��� �� �� �� �� �� �� �� .+��� $��3��#��[ 
��������� $��3��$��0��$��14(�$��Center=�$��Center=�Analog Device Information�� $��0��$��1��#��[ �� $��0��$��1��$��12(�$��Center=�$��5,0,0,0q�Type of Value�� $��0��$��2��#��[ 
��������� $��0��$��2��$��12(�$��Center=�$��5,0,0,0q�Current Value�� $��0��$��3��#��[ �� $��0��$��3��$��12(�$��Center=�$��5,0,0,0q�Setting Value�� $��0��$��4��#��[ 
��������� $��0��$��4��$��12(�$��Center=�$��5,0,0,0q�
Max. Value�� $��0��$��5��#��[ �� $��0��$��5��$��12(�$��Center=�$��5,0,0,0q�Alarm Deviation�� $��0��$��6��#��[ 
��������� $��0��$��6��$��12(�$��Center=�$��5,0,0,0q�Warning Deviation�� $��0��$��7��#��[ �� $��0��$��7��$��12(�$��Center=�$��5,0,0,0q�Ramping Time�� $��0��$��8��#��[ 
��������� $��0��$��8��$��12(�$��Center=�$��5,0,0,0q�Note�� $��1��$��1��#��[ �� $��1��$��1��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��2��#��[ 
��������� $��1��$��2��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��3��#��[ �� $��1��$��3��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��4��#��[ 
��������� $��1��$��4��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��5��#��[ �� $��1��$��5��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��6��#��[ 
��������� $��1��$��6��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��7��#��[ �� $��1��$��7��$��12(�$��Center=�$��5,0,0,0q��� $��1��$��8��#��[ 
��������� $��1��$��8��$��12(�$��Center=�$��5,0,0,0q�