﻿#pragma checksum "..\..\..\CreateBucketPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "037A882ED77AEE05D051DF17888524C22AE3B28E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AwsBuckets;
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


namespace AwsBuckets {
    
    
    /// <summary>
    /// CreateBucketPage
    /// </summary>
    public partial class CreateBucketPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\CreateBucketPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbBucketName;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\CreateBucketPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCreateBucket;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\CreateBucketPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbMessage;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\CreateBucketPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgBucketList;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\CreateBucketPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnReturnToMain;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.9.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AwsBuckets;component/createbucketpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CreateBucketPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.9.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbBucketName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.btnCreateBucket = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\CreateBucketPage.xaml"
            this.btnCreateBucket.Click += new System.Windows.RoutedEventHandler(this.btnCreateBucket_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.tbMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.dgBucketList = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 5:
            this.btnReturnToMain = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\CreateBucketPage.xaml"
            this.btnReturnToMain.Click += new System.Windows.RoutedEventHandler(this.btnReturnToMain_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

