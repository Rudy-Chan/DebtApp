﻿#pragma checksum "..\..\..\Page\Page_OperationLog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "67EC0BDC46716DC2FE90FA40D0F4A4675AA17D50"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
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
using Wpf_Audit;


namespace Wpf_Audit {
    
    
    /// <summary>
    /// Page_OperationLog
    /// </summary>
    public partial class Page_OperationLog : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker Dp_TimeStart;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker Dp_TimeEnd;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Query;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Export;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid Dg_OperationLog;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel proBar;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\Page\Page_OperationLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Lab_Empty;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wpf_Audit;component/page/page_operationlog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Page\Page_OperationLog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Dp_TimeStart = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 2:
            this.Dp_TimeEnd = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.Btn_Query = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\..\Page\Page_OperationLog.xaml"
            this.Btn_Query.Click += new System.Windows.RoutedEventHandler(this.Btn_Query_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Btn_Export = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\Page\Page_OperationLog.xaml"
            this.Btn_Export.Click += new System.Windows.RoutedEventHandler(this.Btn_Export_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Dg_OperationLog = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 6:
            this.proBar = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 7:
            this.Lab_Empty = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

