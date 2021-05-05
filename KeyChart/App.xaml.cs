using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KeyChart.Pages;
using KeyChart.Services;
using KeyChart.Util;

#nullable enable

namespace KeyChart
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static BackgroundTaskDeferral? AppServiceDeferral;
        public static AppServiceConnection? Connection;
        public static bool IsForeground = false;
        

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.LeavingBackground += (_, _) => IsForeground = true;
            this.EnteredBackground += (_, _) => IsForeground = false;

        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (args.TaskInstance.TriggerDetails is not AppServiceTriggerDetails details) return;
            
            // only accept connections from callers in the same package
            if (details.CallerPackageFamilyName != Package.Current.Id.FamilyName) return;
            

            AppServiceDeferral = args.TaskInstance.GetDeferral();
            args.TaskInstance.Canceled += OnTaskCanceled;

            Connection = details.AppServiceConnection;
            Connection.RequestReceived += OnRequestReceived;
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            AppServiceDeferral?.Complete();
            AppServiceDeferral = null;
            Connection = null;
        }

        private void ExtendAcrylicIntoTitleBar()
        {
            // CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame? rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            ApplicationView.TerminateAppOnFinalViewClose = false;

            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += (_, e) =>
            {
                e.Handled = false;
                e.GetDeferral().Complete();
            };

            Debug.WriteLine("================================================================");
            Debug.WriteLine("==== {0:G}", e.Kind);
            Debug.WriteLine("================================================================");

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();

                ExtendAcrylicIntoTitleBar();

                await StartHelperProcess();
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
        }

        private async Task StartHelperProcess()
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                ApplicationData.Current.LocalSettings.Values["processID"] = Process.GetCurrentProcess().Id;
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            if (AppServiceDeferral != null)
            {
                AppServiceDeferral.Complete();
            }
            deferral.Complete();
        }


        private CoreApplicationView? _compactView;

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var dispatcher = CoreApplication.MainView.Dispatcher;
            var messageDeferral = args.GetDeferral();
            var action = (HotkeyService.HotkeyAction)args.Request.Message["Action"];
            switch (action)
            {
                case HotkeyService.HotkeyAction.ShowApp:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Application.Current.Exit();
                        //stingrayMove.Begin();
                    });
                    break;
                case HotkeyService.HotkeyAction.ShowOverlay:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (Current is not App app) return;



                        if (_compactView is {})
                        {
                            var compactDispatcher = _compactView.Dispatcher;
                            _compactView = null;
                            await compactDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                async () => await ApplicationView.GetForCurrentView().TryConsolidateAsync());
                            //await viewRef.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            //{
                            //    viewRef.Dispatcher.StopProcessEvents();
                            //    viewRef.CoreWindow.Close();
                            //});

                            return;
                        }

                        if (await KeyboardHelper.GetSelectedKeyboardSize() is not { } keyboardSize) return;

                        int compactViewId = 0;
                        if (IsForeground)
                        {
                            _compactView = CoreApplication.CreateNewView();
                        }
                        else
                        {
                            _compactView = CoreApplication.GetCurrentView();
                        }


                        await _compactView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            var frame = new Frame();

                            frame.Navigate(typeof(ChartPage));
                            Window.Current.Content = frame;
                            Window.Current.Activate();
                            ApplicationView.GetForCurrentView().Title = "CompactOverlay Window";
                            compactViewId = ApplicationView.GetForCurrentView().Id;
                        });


                        if (_compactView is null) return;
                        var prefs = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);

                        var kbWithPad = new Size(keyboardSize.Width + 12, keyboardSize.Height + 12);
                        prefs.CustomSize = new Size(500, (500 / kbWithPad.Width) * kbWithPad.Height);
                        prefs.ViewSizePreference = ViewSizePreference.UseHalf;
                        await ApplicationViewSwitcher.TryShowAsViewModeAsync(compactViewId,
                            ApplicationViewMode.CompactOverlay, prefs);
                    });
                    break;
                default:
                    break;
            }
            await args.Request.SendResponseAsync(new ValueSet());
            messageDeferral.Complete();

            // we no longer need the connection
            App.AppServiceDeferral?.Complete();
            App.Connection = null;
        }

    }
}
