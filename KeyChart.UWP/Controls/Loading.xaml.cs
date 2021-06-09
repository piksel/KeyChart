using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
#nullable enable

namespace KeyChart.Controls
{
    public sealed partial class Loading : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(Loading), new PropertyMetadata("Loading..."));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading", typeof(string), typeof(Loading), new PropertyMetadata(false));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "Padding", typeof(Thickness), typeof(Loading), new PropertyMetadata(new Thickness(12)));

        public static readonly DependencyProperty RingForegroundProperty = DependencyProperty.Register(
            "RingForeground", typeof(Brush), typeof(Loading), new PropertyMetadata(null));

        public Brush? RingForeground
        {
            get => (Brush) GetValue(RingForegroundProperty);
            set => SetValue(RingForegroundProperty, value);
        }

        public new Thickness Padding
        {
            get => (Thickness) GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public Loading()
        {
        //    this.PaddingProperty 
            this.InitializeComponent();
        }
    }
}
