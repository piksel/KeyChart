using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Hosting;
using KeyChart.Keyboards.QMK;
using KeyChart.Pages;
using muic = Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.UI.Core;
using KeyChart.Keyboards;
using KeyChart.Services;
using KeyChart.Util;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KeyChart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(LayerCollection), typeof(MainPage), null);
        public KeyboardModel Model
        {
            get => (KeyboardModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }



        public MainPage()
        {
            this.InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += (tb, _) =>
            {
                AppTitleBar.Height = tb.Height;
               
            };
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(KeyboardHelper.GetSelectedKeyboardModel() is null ? typeof(KeyboardPage) : typeof(ChartPage));
        }

       

        private void UpdateAppTitleMargin(muic.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitle.TranslationTransition = new Vector3Transition();

                if ((sender.DisplayMode == muic.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == muic.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitle.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitle.Margin;

                if ((sender.DisplayMode == muic.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == muic.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }
        }

        private void NavigationView_PaneOpened(muic.NavigationView sender, object _) => UpdateAppTitleMargin(sender);
        private void NavigationView_PaneClosing(muic.NavigationView sender, muic.NavigationViewPaneClosingEventArgs _) => UpdateAppTitleMargin(sender);

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NavToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SideNav.IsPaneOpen = !SideNav.IsPaneOpen;
        }

        private async void SideNav_OnSelectionChanged(muic.NavigationView sender, muic.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationFrame.Navigate(typeof(SettingsPage));
            }
            else if(args.SelectedItem is muic.NavigationViewItem {Tag: string tag})
            {
                switch (tag)
                {
                    case "keyboard":
                        NavigationFrame.Navigate(typeof(KeyboardPage));
                        break;
                    case "chart":
                        NavigationFrame.Navigate(typeof(ChartPage));
                        break;
                    case "fullscreen":
                        var view = ApplicationView.GetForCurrentView();
                        if (view.IsFullScreenMode)
                        {
                            view.ExitFullScreenMode();
                        }
                        else
                        {
                            view.TryEnterFullScreenMode();
                        }

                        break;
                    default:
                        await new MessageDialog($"Unknown page \"{tag}\"", "Navigation error").ShowAsync();
                        break;
                }
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }
    }
}
