﻿#pragma checksum "..\..\..\..\Controls\DeviceSelect.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C2FBD18365AAE6B5FF615DBD9612A8ADA85D58C4"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;
using Microsoft.Xaml.Behaviors.Input;
using Microsoft.Xaml.Behaviors.Layout;
using Microsoft.Xaml.Behaviors.Media;
using SapphireXR_App.Controls;
using SapphireXR_App.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SapphireXR_App.Controls {
    
    
    /// <summary>
    /// DeviceSelect
    /// </summary>
    public partial class DeviceSelect : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox LbxLeft;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CbSelectPlotTagPV;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CbSelectPlotTagSV;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CbSelectPlotTagEtc;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMoveRightPlotTag;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMoveLeftPlotTag;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMoveRightAllPlotTag;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMoveLeftAllPlotTag;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\Controls\DeviceSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox LbxRight;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SapphireXR_App;component/controls/deviceselect.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\DeviceSelect.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LbxLeft = ((System.Windows.Controls.ListBox)(target));
            return;
            case 2:
            this.CbSelectPlotTagPV = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 3:
            this.CbSelectPlotTagSV = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.CbSelectPlotTagEtc = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.btnMoveRightPlotTag = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.btnMoveLeftPlotTag = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.btnMoveRightAllPlotTag = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.btnMoveLeftAllPlotTag = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.LbxRight = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

