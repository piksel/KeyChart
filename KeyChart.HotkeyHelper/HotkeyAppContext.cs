using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using KeyChart.Services;
using Action = KeyChart.Services.HotkeyService.HotkeyAction;

namespace KeyChart.HotkeyHelper
{
    class HotkeyAppContext : ApplicationContext
    {
        private readonly HotKeyWindow _hotkeyWindow;
        private readonly Process _process;

        public HotkeyAppContext()
        {
#if false
            var processId = Program.Rpc.ProcessId;
            _process = Process.GetProcessById(processId);
            _process.EnableRaisingEvents = true;
#if DEBUG
            _process.Exited += (_, __) => Application.Exit();
#endif
#endif
            _hotkeyWindow = new HotKeyWindow();
            _hotkeyWindow.HotkeyPressed += Program.Rpc.HotkeyPressed;
            //_hotkeyWindow.HotkeyPressed += HotkeyPressed_UWP;
            _hotkeyWindow.RegisterCombo(Action.ShowApp, KeyModifiers.Control, Keys.Home);
            _hotkeyWindow.RegisterCombo(Action.ShowOverlay, KeyModifiers.None, Keys.Pause);
        }
        
    }
}
