using System;

namespace KeyChart.Services
{
    public partial class HotkeyService
    {
        public static readonly string Name = nameof(HotkeyService);

        public enum HotkeyAction: int
        {
            ShowOverlay = 1001,
            ShowApp = 1002
        }

        public static readonly string Address = "http://localhost:38429";
    }
}
