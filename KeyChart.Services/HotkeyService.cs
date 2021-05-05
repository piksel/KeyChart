using System;

namespace KeyChart.Services
{
    public static class HotkeyService
    {
        public static readonly string Name = nameof(HotkeyService);

        public enum HotkeyAction: int
        {
            ShowOverlay = 1001,
            ShowApp = 1002
        }
    }
}
