using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using KeyChart.GUI.Platforms.Windows.Winterop;
using KeyChart.GUI.Util;

namespace KeyChart.GUI.Platforms.Windows
{
  public class WindowsTrayIcon : ITrayIcon
  {
    public ICommand DoubleClickCommand { get; set; }

    public ICommand BalloonClickedCommand { get; set; }

    private TaskBarIcon? taskBarIcon;

    private object lastBalloonState;
    
    public WindowIcon Icon { get; set; }
    public string TooltipText { get; set; }
    public ContextMenu ContextMenu { get; set; }
    
    public HotkeyWindowImplWin32 Window { get; set; }

    public void SetIcon()
    {
        using var ms = new MemoryStream();
        Icon.PlatformImpl.Save(ms);
        ms.Seek(0, SeekOrigin.Begin);


        var icon = new Icon(ms);
        taskBarIcon = new TaskBarIcon(Window, icon, TooltipText);
        taskBarIcon.MouseEventHandler += TaskBarIconOnMouseEventHandler;
    }

    

    public void ShowBalloon(string text, object state)
    {
      this.lastBalloonState = state;
      taskBarIcon?.ShowBalloonTip(TooltipText, text, BalloonFlags.Info);
    }

    public void SetMenu(IEnumerable<PopupMenuItem> menuItems)
    {
        // TODO: Fix this HORRIBLE API
        if(taskBarIcon is null) return;
        taskBarIcon.PopupMenu.MenuItems = menuItems.ToList();
        taskBarIcon.PopupMenu.BuildMenu();
    }
    

    private void TaskBarIconOnMouseEventHandler(object? sender, MouseEvent e)
    {
      if (e == MouseEvent.IconDoubleClick)
      {
        if (DoubleClickCommand is {} command && command.CanExecute(null))
        {
          command.Execute(null);
        }
      }

      if (e == MouseEvent.BalloonToolTipClicked)
      {
        if (BalloonClickedCommand is {} command && command.CanExecute(lastBalloonState))
        {
          command.Execute(lastBalloonState);
        }
      }
    }

    public void Dispose()
    {
      if (taskBarIcon != null)
        taskBarIcon.MouseEventHandler -= TaskBarIconOnMouseEventHandler;
      taskBarIcon?.Dispose();
    }
  }
}
