﻿#pragma checksum "..\..\Page_File.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D8868E70959B396F4951BE79796D1A790FC00D82"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Debt;
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


namespace Debt {
    
    
    /// <summary>
    /// Page_File
    /// </summary>
    public partial class Page_File : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Skin;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Min;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Max;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Close;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Upload_Ctl;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_View_Ctl;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Download_Ctl;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\Page_File.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContentControl Dynamic_Page;
        
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
            System.Uri resourceLocater = new System.Uri("/Debt;component/page_file.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Page_File.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            
            #line 9 "..\..\Page_File.xaml"
            ((Debt.Page_File)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 17 "..\..\Page_File.xaml"
            ((System.Windows.Controls.Border)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Heading_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Btn_Skin = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\Page_File.xaml"
            this.Btn_Skin.Click += new System.Windows.RoutedEventHandler(this.Btn_Skin_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Btn_Min = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\Page_File.xaml"
            this.Btn_Min.Click += new System.Windows.RoutedEventHandler(this.Btn_Min_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Btn_Max = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\Page_File.xaml"
            this.Btn_Max.Click += new System.Windows.RoutedEventHandler(this.Btn_Max_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Btn_Close = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\Page_File.xaml"
            this.Btn_Close.Click += new System.Windows.RoutedEventHandler(this.Btn_Close_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Btn_Upload_Ctl = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\Page_File.xaml"
            this.Btn_Upload_Ctl.Click += new System.Windows.RoutedEventHandler(this.Btn_Upload_Ctl_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Btn_View_Ctl = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\Page_File.xaml"
            this.Btn_View_Ctl.Click += new System.Windows.RoutedEventHandler(this.Btn_View_Ctl_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Btn_Download_Ctl = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\Page_File.xaml"
            this.Btn_Download_Ctl.Click += new System.Windows.RoutedEventHandler(this.Btn_Download_Ctl_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Dynamic_Page = ((System.Windows.Controls.ContentControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

