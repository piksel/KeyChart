using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Piksel.UWP.Dialogs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Piksel.UWP.ExampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static readonly DependencyProperty RenameResultProperty = DependencyProperty.Register(
            "RenameResult", typeof(string), typeof(MainPage), new PropertyMetadata("Thingy McThingface"));

        public string RenameResult
        {
            get => (string) GetValue(RenameResultProperty);
            set => SetValue(RenameResultProperty, value);
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (await RenameDialog.Show(RenameResult, "Rename example:") is string newName)
            {
                RenameResult = newName;
            }
        }
    }
}
