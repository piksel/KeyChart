using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Grpc.Net.Client;
using KeyChart.Services;
using Microsoft.Windows.Sdk;
using Action = KeyChart.Services.HotkeyService.HotkeyAction;

namespace KeyChart.HotkeyHelper
{
    public class AppRPC
    {
        private bool _hotkeyInProgress;
        
        public async void HotkeyPressed(Action action)
        {
            if (_hotkeyInProgress) return; // prevent re-entrance (not thread safe, but probably close enough)
            _hotkeyInProgress = true;

            try
            {
                Debug.WriteLine("[KeyChart.HotkeyHelper] Got Hotkey action: {0:G}", action);
                
                if (action == Action.ShowApp)
                {
                    Application.Exit();
                }

                using var channel = GrpcChannel.ForAddress(HotkeyService.Address, new GrpcChannelOptions
                {

                });
                var client = new HotkeyService.HotkeyServiceClient(channel);

                var resp = await client.ShowWindowAsync(new ShowWindowArgs()
                {
                    DisplayNum = 0,
                    FullScreen = true,
                });
                
                Debug.WriteLine(resp);

                // "Avalonia-4ebe60ac-4a8b-4acb-97c1-fc5e00d0a804"
                // var window = PInvoke.FindWindow(null, "KeyChart.Overview");
                // if (window.Value == 0)
                // {
                //     MessageBox.Show("Could not find window!");
                //     return;
                // }

                // PInvoke.ShowWindow(window, SHOW_WINDOW_CMD.SW_MAX);
                //PInvoke.AnimateWindow(window, 


            }
            catch (Exception x)
            {
                Debug.WriteLine("[KeyChart.HotkeyHelper] Exception: {0}", x);
                _hotkeyInProgress = false;
            }
        }

        public int ProcessId => 0;

        public string MutexName => "se.piksel.keychart.avalonia_HotkeyHelperMutex";
    }
}