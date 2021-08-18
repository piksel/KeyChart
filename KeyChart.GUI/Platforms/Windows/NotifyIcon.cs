using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Win32;
using KeyChart.GUI.Platforms.Windows.Winterop;
using KeyChart.GUI.Util;

namespace KeyChart.GUI.Platforms.Windows
{
  
  /// <summary>
  /// A WPF proxy to for a taskbar icon (NotifyIcon) that sits in the system's
  /// taskbar notification area ("system tray").
  /// </summary>
  public class TaskBarIcon : IDisposable
  {
    private readonly object lockObject = new object();

    #region Members

    /// <summary>
    /// Represents the current icon data.
    /// </summary>
    private Winterop.NotifyIconData iconData;

    private bool isDoubleClick;

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsTaskbarIconCreated { get; private set; }

    public Icon Icon { get; }

    public PopupMenu PopupMenu { get; }

    public event EventHandler<MouseEvent>? MouseEventHandler;

    #endregion

    #region Construction

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    public TaskBarIcon(HotkeyWindowImplWin32 window, Icon icon, string tooltipText)
    {
      Icon = icon;

      // init icon data structure
      iconData = NotifyIconData.CreateDefault(window.Handle.Handle);
      iconData.IconHandle = Icon?.Handle ?? IntPtr.Zero;
      iconData.ToolTipText = tooltipText;

      // create the taskbar icon
      CreateTaskbarIcon();

      // register event listeners
      //messageSink.MouseEventReceived += OnMouseEvent;
      //messageSink.TaskbarCreated += OnTaskbarCreated;
      window.CustomWndProc += WindowOnCustomWndProc;
      PopupMenu = new PopupMenu(window.Handle.Handle);

      // register listener in order to get notified when the application closes
      if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        lifetime.Exit += OnExit;
      }
    }

    #endregion
    
    private void WindowOnCustomWndProc(object? sender, CustomWndProcEventArgs e)
    {

        if (e.Message == WindowMessage.WM_COMMAND)
        {
            Debug.WriteLine($"Got command: {e.WParam}");
            var menuItem = PopupMenu.MenuItems[e.WParam.ToInt32()];
            

            Debug.WriteLine($"Menu item: {menuItem.Text}");
            menuItem.Selected.Invoke();
            return;
        }
        
        if ((uint)e.Message != iconData.CallbackMessageId) return;

        var message = (WindowMessage) e.LParam.ToInt32();
        switch (message)
        {
            case WindowMessage.WM_CONTEXTMENU:
                // TODO: Handle WM_CONTEXTMENU, see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                Debug.WriteLine("Unhandled WM_CONTEXTMENU");
                break;

            case WindowMessage.WM_MOUSEMOVE:
              // Skips since it's not handled
              // OnMouseEvent(MouseEvent.MouseMove);
              break;

            case WindowMessage.WM_LBUTTONDOWN:
              OnMouseEvent(MouseEvent.IconLeftMouseDown);
                break;

            case WindowMessage.WM_LBUTTONUP:
                if (!isDoubleClick)
                {
                  OnMouseEvent(MouseEvent.IconLeftMouseUp);
                }
                isDoubleClick = false;
                break;

            case WindowMessage.WM_LBUTTONDBLCLK:
                isDoubleClick = true;
                OnMouseEvent(MouseEvent.IconDoubleClick);
                break;

            case WindowMessage.WM_RBUTTONDOWN:
              OnMouseEvent(MouseEvent.IconRightMouseDown);
                break;

            case WindowMessage.WM_RBUTTONUP:
                var (x, y) = CursorPos.Current;
                PopupMenu.ShowAt(x, y);
                
              //OnMouseEvent(MouseEvent.IconRightMouseUp);
                break;

            case WindowMessage.WM_RBUTTONDBLCLK:
                //double click with right mouse button - do not trigger event
                break;

            case WindowMessage.WM_MBUTTONDOWN:
              OnMouseEvent(MouseEvent.IconMiddleMouseDown);
                break;

            case WindowMessage.WM_MBUTTONUP:
              OnMouseEvent(MouseEvent.IconMiddleMouseUp);
                break;

            case WindowMessage.WM_MBUTTONDBLCLK:
                //double click with middle mouse button - do not trigger event
                break;

            case WindowMessage.NIN_BALLOONSHOW:
                OnBalloonToolTipChanged(true);
                break;

            case WindowMessage.NIN_BALLOONHIDE:
            case WindowMessage.NIN_BALLOONTIMEOUT:
              OnBalloonToolTipChanged(false);
                break;

            case WindowMessage.NIN_BALLOONUSERCLICK:
              OnMouseEvent(MouseEvent.BalloonToolTipClicked);
                break;

            case WindowMessage.NIN_POPUPOPEN:
                OnChangeToolTipStateRequest(true);
                break;

            case WindowMessage.NIN_POPUPCLOSE:
              OnChangeToolTipStateRequest(false);
                break;

            case WindowMessage.NIN_SELECT:
                // TODO: Handle NIN_SELECT see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                Debug.WriteLine("Unhandled NIN_SELECT");
                break;

            case WindowMessage.NIN_KEYSELECT:
                // TODO: Handle NIN_KEYSELECT see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                Debug.WriteLine("Unhandled NIN_KEYSELECT");
                break;

            default:
                Debug.WriteLine("Unhandled NotifyIcon message ID: " + e.LParam);
                break;
        }
    }

    private void OnChangeToolTipStateRequest(bool open)
    {
      // 
    }

    private void OnBalloonToolTipChanged(bool showing)
    {
      //
    }

    #region Process Incoming Mouse Events

    /// <summary>
    /// Processes mouse events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="me">Event flag.</param>
    private void OnMouseEvent(MouseEvent me)
    {
      if (IsDisposed)
        return;

      switch (me)
      {
        case MouseEvent.MouseMove:
          // immediately return - there's nothing left to evaluate
          return;
        case MouseEvent.IconRightMouseDown:
        case MouseEvent.IconLeftMouseDown:
        case MouseEvent.IconRightMouseUp:
        case MouseEvent.IconLeftMouseUp:
        case MouseEvent.IconMiddleMouseDown:
        case MouseEvent.IconMiddleMouseUp:
        case MouseEvent.BalloonToolTipClicked:
        case MouseEvent.IconDoubleClick:
          MouseEventHandler?.Invoke(this, me);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(me), "Missing handler for mouse event flag: " + me);
      }
    }

    #endregion

    #region Balloon Tips

    /// <summary>
    /// Displays a balloon tip with the specified title,
    /// text, and icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="symbol">A symbol that indicates the severity.</param>
    public void ShowBalloonTip(string title, string message, BalloonFlags symbol)
    {
      lock (lockObject)
      {
        ShowBalloonTip(title, message, symbol, IntPtr.Zero);
      }
    }


    /// <summary>
    /// Invokes <see cref="WinApi.Shell_NotifyIcon"/> in order to display
    /// a given balloon ToolTip.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="flags">Indicates what icon to use.</param>
    /// <param name="balloonIconHandle">A handle to a custom icon, if any, or
    /// <see cref="IntPtr.Zero"/>.</param>
    private void ShowBalloonTip(string title, string message, BalloonFlags flags, IntPtr balloonIconHandle)
    {
      EnsureNotDisposed();

      iconData.BalloonText = message ?? string.Empty;
      iconData.BalloonTitle = title ?? string.Empty;

      iconData.BalloonFlags = flags;
      iconData.CustomBalloonIconHandle = balloonIconHandle;
      WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info | IconDataMembers.Icon);
    }


    /// <summary>
    /// Hides a balloon ToolTip, if any is displayed.
    /// </summary>
    public void HideBalloonTip()
    {
      EnsureNotDisposed();

      // reset balloon by just setting the info to an empty string
      iconData.BalloonText = iconData.BalloonTitle = string.Empty;
      WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info);
    }

    #endregion

    #region Create / Remove Taskbar Icon

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
    private void OnTaskbarCreated()
    {
      IsTaskbarIconCreated = false;
      CreateTaskbarIcon();
    }


    /// <summary>
    /// Creates the taskbar icon. This message is invoked during initialization,
    /// if the taskbar is restarted, and whenever the icon is displayed.
    /// </summary>
    private void CreateTaskbarIcon()
    {
      lock (lockObject)
      {
        if (IsTaskbarIconCreated)
        {
          return;
        }

        const IconDataMembers members = IconDataMembers.Message | IconDataMembers.Icon | IconDataMembers.Tip;

        //write initial configuration
        var status = WriteIconData(ref iconData, NotifyCommand.Add, members);
        if (!status)
        {
          // couldn't create the icon - we can assume this is because explorer is not running (yet!)
          // -> try a bit later again rather than throwing an exception. Typically, if the windows
          // shell is being loaded later, this method is being re-invoked from OnTaskbarCreated
          // (we could also retry after a delay, but that's currently YAGNI)
          return;
        }

        // messageSink.Version = (NotifyIconVersion)iconData.VersionOrTimeout;

        IsTaskbarIconCreated = true;
      }
    }

    /// <summary>
    /// Closes the taskbar icon if required.
    /// </summary>
    private void RemoveTaskbarIcon()
    {
      lock (lockObject)
      {
        // make sure we didn't schedule a creation

        if (!IsTaskbarIconCreated)
        {
          return;
        }

        WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);
        IsTaskbarIconCreated = false;
      }
    }

    #endregion

    #region Dispose / Exit

    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }


    /// <summary>
    /// Checks if the object has been disposed and
    /// raises a <see cref="ObjectDisposedException"/> in case
    /// the <see cref="IsDisposed"/> flag is true.
    /// </summary>
    private void EnsureNotDisposed()
    {
      if (IsDisposed)
        throw new ObjectDisposedException(GetType().FullName);
    }


    /// <summary>
    /// Disposes the class if the application exits.
    /// </summary>
    private void OnExit(object? sender, EventArgs e)
    {
      Dispose();
    }


    /// <summary>
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from this class.
    /// </para>
    /// </summary>
    ~TaskBarIcon()
    {
      Dispose(false);
    }


    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <remarks>This method is not virtual by design. Derived classes
    /// should override <see cref="Dispose(bool)"/>.
    /// </remarks>
    public void Dispose()
    {
      Dispose(true);

      // This object will be cleaned up by the Dispose method.
      // Therefore, you should call GC.SuppressFinalize to
      // take this object off the finalization queue 
      // and prevent finalization code for this object
      // from executing a second time.
      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Closes the tray and releases all resources.
    /// </summary>
    /// <summary>
    /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
    /// If disposing equals <c>true</c>, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// </summary>
    /// <param name="disposing">If disposing equals <c>false</c>, the method
    /// has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can
    /// be disposed.</param>
    /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
    /// the method has already been called.</remarks>
    private void Dispose(bool disposing)
    {
      // don't do anything if the component is already disposed
      if (IsDisposed || !disposing)
        return;

      lock (lockObject)
      {
        IsDisposed = true;

        // de-register application event listener
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
          lifetime.Exit -= OnExit;
        }

        // dispose message sink
        //messageSink.Dispose();

        // remove icon
        RemoveTaskbarIcon();
      }
    }

    #endregion

    #region WriteIconData
    
    /// <summary>
    /// Updates the taskbar icons with data provided by a given
    /// <see cref="NotifyIconData"/> instance.
    /// </summary>
    /// <param name="data">Configuration settings for the NotifyIcon.</param>
    /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
    /// <param name="flags">Defines which members of the <paramref name="data"/>
    /// structure are set.</param>
    /// <returns>True if the data was successfully written.</returns>
    /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
    private bool WriteIconData(ref Winterop.NotifyIconData data, NotifyCommand command, IconDataMembers flags)
    {
      data.ValidMembers |= flags;
      lock (lockObject)
      {
        return WinApi.Shell_NotifyIcon(command, ref data);
      }
    }

    #endregion
    
    /// <summary>
    /// Reads a given image resource into a WinForms icon.
    /// </summary>
    /// <param name="imageSource">Image source pointing to
    /// an icon file (*.ico).</param>
    /// <returns>An icon object that can be used with the
    /// taskbar area.</returns>
    public static Icon? ToIcon(string? imageSource)
    {
      if (imageSource == null)
        return null;

      var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
      if (!executingAssembly.GetManifestResourceNames().ToList().Contains(imageSource)) return null;
      var stream = executingAssembly.GetManifestResourceStream(imageSource);
      return new Icon(stream);

    }
  }
}