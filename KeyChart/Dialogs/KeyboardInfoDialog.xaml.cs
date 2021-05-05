using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KeyChart.Keyboards;
using KeyChart.Keyboards.QMK;
using muxc = Microsoft.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

#nullable enable

namespace KeyChart.Dialogs
{
    public sealed partial class KeyboardInfoDialog : ContentDialog
    {
        public static readonly DependencyProperty KeyboardModelProperty = DependencyProperty.Register(
            "KeyboardModel", typeof(KeyboardModel), typeof(KeyboardInfoDialog), new PropertyMetadata(default(KeyboardModel?)));

        public KeyboardModel? KeyboardModel
        {
            get => (KeyboardModel?) GetValue(KeyboardModelProperty);
            set => SetValue(KeyboardModelProperty, value);
        }

        public KeyboardInfoDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        public static async Task ShowDialog(KeyboardModel keyboard)
        {
            var dialog = new KeyboardInfoDialog()
            {
                KeyboardModel = keyboard
            };

            await dialog.ShowAsync();
        }

        private async void Nav_OnSelectionChanged(muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not  muxc.NavigationViewItem nvi) return;
            if (KeyboardModel is null) return;

            switch (nvi.Tag as string)
            {
                case "readme":
                    KeyboardReadme.Text = await QmkClient.FetchKeyboardReadme(KeyboardModel.KeyMap.Keyboard);
                    ReadmeLoading.IsLoading = false;
                    break;
                default:
                    break;
            }
        }

        private async void KeyboardInfoDialog_OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (KeyboardModel?.Info.Url is not {} targetUrl) return;
            await Windows.System.Launcher.LaunchUriAsync(new Uri(targetUrl));
        }
    }
}
