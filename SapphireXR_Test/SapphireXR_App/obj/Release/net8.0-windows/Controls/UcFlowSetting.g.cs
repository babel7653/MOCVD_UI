﻿#pragma checksum "..\..\..\..\Controls\UcFlowSetting.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BB3B86E966FC13CF61068C6E4DACE376C4E69F13"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using SapphireXR_App.Controls;
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
    /// UcFlowSetting
    /// </summary>
    public partial class UcFlowSetting : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SapphireXR_App.Controls.UcFlowSetting ucfs;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ucTargetValue;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox usRampTime;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox usDeviation;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox usCurrentValue;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox usControlValue;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\Controls\UcFlowSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox usMaxValue;
        
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
            System.Uri resourceLocater = new System.Uri("/SapphireXR_App;component/controls/ucflowsetting.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\UcFlowSetting.xaml"
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
            this.ucfs = ((SapphireXR_App.Controls.UcFlowSetting)(target));
            
            #line 8 "..\..\..\..\Controls\UcFlowSetting.xaml"
            this.ucfs.Loaded += new System.Windows.RoutedEventHandler(this.ucfs_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ucTargetValue = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.usRampTime = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.usDeviation = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.usCurrentValue = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.usControlValue = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.usMaxValue = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            
            #line 76 "..\..\..\..\Controls\UcFlowSetting.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OK_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 80 "..\..\..\..\Controls\UcFlowSetting.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Cancel_Click_1);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

