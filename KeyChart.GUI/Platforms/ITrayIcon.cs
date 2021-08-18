using System.Collections.Generic;
using Avalonia.Controls;

namespace KeyChart.GUI.Platforms
{
    public interface ITrayIcon : System.IDisposable
    {
        System.Windows.Input.ICommand? DoubleClickCommand { get; set; }

        System.Windows.Input.ICommand? BalloonClickedCommand { get; set; }
        ContextMenu? ContextMenu { get; set; }

        void SetIcon();

        void ShowBalloon(string text, object state);

        void SetMenu(IEnumerable<PopupMenuItem> menuItems);
    }
}
