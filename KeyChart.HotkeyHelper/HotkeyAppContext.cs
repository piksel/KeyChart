using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Storage;
using KeyChart.Services;
using Action = KeyChart.Services.HotkeyService.HotkeyAction;

namespace KeyChart.HotkeyHelper
{
    class HotkeyAppContext : ApplicationContext
    {
        private readonly HotKeyWindow _hotkeyWindow;
        private readonly Process _process;
        private bool _hotkeyInProgress;

        public HotkeyAppContext()
        {
            var processId = (int)ApplicationData.Current.LocalSettings.Values["processId"];
            _process = Process.GetProcessById(processId);
            _process.EnableRaisingEvents = true;
#if DEBUG
            _process.Exited += (_, __) => Application.Exit();
#endif
            _hotkeyWindow = new HotKeyWindow();
            _hotkeyWindow.HotkeyPressed += HotkeyPressed;
            _hotkeyWindow.RegisterCombo(Action.ShowApp, KeyModifiers.Control, Keys.Home);
            _hotkeyWindow.RegisterCombo(Action.ShowOverlay, KeyModifiers.None, Keys.Pause);
        }

        private async void HotkeyPressed(Action action)
        {
            if (_hotkeyInProgress) return; // prevent re-entrance (not thread safe, but probably close enough)
            _hotkeyInProgress = true;

            try
            {
                Debug.WriteLine("[KeyChart.HotkeyHelper] Got Hotkey action: {0:G}", action);

                // bring the UWP to the foreground (optional)
                IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
                foreach (var entry in appListEntries)
                {
                    Debug.WriteLine("Id: {0}, Name: {1}, Descr: {2}", entry.AppUserModelId,
                        entry.DisplayInfo.DisplayName, entry.DisplayInfo.Description);
                }
                await appListEntries.First().LaunchAsync();

                // send the key ID to the UWP
                ValueSet hotkeyPressed = new ValueSet() {{"Action", (int) action}};

                var connection = new AppServiceConnection()
                {
                    PackageFamilyName = Package.Current.Id.FamilyName,
                    AppServiceName = HotkeyService.Name
                };

                var status = await connection.OpenAsync();
                if (status != AppServiceConnectionStatus.Success)
                {
                    Debug.WriteLine("[KeyChart.HotkeyHelper] Got AppService status: {0}", status);
                    Application.Exit();
                }

                connection.ServiceClosed += (sender, args) =>
                {
                    Debug.WriteLine("[KeyChart.HotkeyHelper] Event Connection.ServiceClosed");
                    _hotkeyInProgress = false;
                };

                await connection.SendMessageAsync(hotkeyPressed);

                if (action == Action.ShowApp)
                {
                    Application.Exit();
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine("[KeyChart.HotkeyHelper] Exception: {0}", x);
                _hotkeyInProgress = false;
            }
        }
    }
}
