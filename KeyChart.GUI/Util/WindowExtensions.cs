using Avalonia.Controls;

namespace KeyChart.GUI.Util
{
    public static class WindowExtensions
    {
        public static void ToggleMinimized(this Window w)
        {
            if (w.WindowState == WindowState.Minimized)
            {
                w.WindowState = WindowState.Normal;
                w.Show();
                w.Activate();
            }
            else
            {
                w.WindowState = WindowState.Minimized;
            }
        }
    }
}