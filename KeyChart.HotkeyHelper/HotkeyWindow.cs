using System.Collections.Generic;
using System.Windows.Forms;
using KeyChart.Services;
using Action = KeyChart.Services.HotkeyService.HotkeyAction;

namespace KeyChart.HotkeyHelper
{
    public sealed class HotKeyWindow : NativeWindow
    {
        

        private readonly List<Action> _registeredActions = new List<Action>();
        public delegate void HotkeyDelegate(Action action);
        public event HotkeyDelegate? HotkeyPressed;

        // creates a headless Window to register for and handle WM_HOTKEY
        public HotKeyWindow()
        {
            CreateHandle(new CreateParams());

            Application.ApplicationExit += (_, __) => DestroyHandle();
        }

        public void RegisterCombo(HotkeyService.HotkeyAction action, KeyModifiers modifiers, Keys vlc)
        {
            if (Winterop.RegisterHotKey(Handle, (int)action, (int)modifiers, (int)vlc))
            {
                _registeredActions.Add(action);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Winterop.WM_HOTKEY: //raise the HotkeyPressed event
                    HotkeyPressed?.Invoke((Action)m.WParam.ToInt32());
                    break;

                case Winterop.WM_DESTROY: //unregister all hot keys
                    foreach (var action in _registeredActions)
                    {
                        Winterop.UnregisterHotKey(Handle, (int)action);
                    }
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
