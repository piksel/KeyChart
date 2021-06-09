using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
#pragma warning disable 414
#pragma warning disable 649

namespace KeyChart.GUI.Platforms.Windows.Winterop
{
    public class PopupMenu
    {
        public PopupMenu(IntPtr ownerWindow)
        {
            ownerHandle = ownerWindow;
        }
        
        private IntPtr menuHandle;
        public List<PopupMenuItem> MenuItems { get; set; }

        uint itemIndexBase = 100;
        private readonly IntPtr ownerHandle;

        public void BuildMenu()
        {
            if (menuHandle != IntPtr.Zero)
            {
                Debug.Assert(DestroyPopupMenu(menuHandle));
            }

            menuHandle = CreatePopupMenu();
            Debug.Assert(menuHandle != IntPtr.Zero);

            for (var index = 0; index < MenuItems.Count; index++)
            {
                var menuItem = MenuItems[index];
                var separator = menuItem == PopupMenuItem.Separator;
                if (AppendMenu(menuHandle, separator ? MenuFlag.MF_SEPARATOR : MenuFlag.MF_STRING, index,
                    menuItem.Text)) continue;
                
                var hres = Marshal.GetHRForLastWin32Error();
                var ex = Marshal.GetExceptionForHR(hres);
                Debug.WriteLine("Failed to append menu item. Handle is 0x{0:x}, error is: 0x{1:x}, hres: 0x{2:x}, exception: {3}", 
                    menuHandle, Marshal.GetLastWin32Error(), hres, ex);
                // var mii = new MenuItemInfo()
                // {
                //     fMask = MenuItemMask.MIIM_ID | MenuItemMask.MIIM_TYPE | MenuItemMask.MIIM_STATE,
                //     wID = itemIndexBase + (uint)index,
                //     fType = separator ? MenuFlag.MF_SEPARATOR : MenuFlag.MF_STRING,
                //     dwTypeData = text,
                //     fState = MenuFlag.MF_ENABLED,
                // };
                // if (!InsertMenuItem(menuHandle, (uint) index, 1, ref mii))
                // {
                //     var hres = Marshal.GetHRForLastWin32Error();
                //     var ex = Marshal.GetExceptionForHR(hres);
                //     Debug.WriteLine("Failed to insert menu item. Handle is 0x{0:x}, error is: 0x{1:x}, hres: 0x{2:x}, exception: {3}", menuHandle, Marshal.GetLastWin32Error(), hres, ex);
                // }
                // menuItem.
            }
        }
        
        
        [DllImport("user32.dll")]
        static extern IntPtr CreatePopupMenu();
        
        [DllImport("user32.dll")]
        static extern bool DestroyPopupMenu(IntPtr hMenu);
                
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InsertMenuItem(IntPtr hMenu, uint uposition, uint uflags, ref MenuItemInfo mii);

        [DllImport("user32")]
        internal static extern bool AppendMenu(IntPtr hMenu, MenuFlag uflags, int uIDNewItemOrSubmenu, string text);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InsertMenu(IntPtr hMenu, int position, MenuFlag uflags, IntPtr uIDNewItemOrSubmenu, string text);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern int SetMenuItemBitmaps(IntPtr hMenu, int nPosition, MenuFlag uflags, IntPtr hBitmapUnchecked, IntPtr hBitmapChecked);
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TrackPopupMenuEx(IntPtr hmenu, TPM fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        public void ShowAt(int x, int y)
        {
            Debug.WriteLine($"Cursor pos: {x}, {y}");
            var result = TrackPopupMenuEx(menuHandle, TPM.LEFTALIGN | TPM.RIGHTALIGN /*| TPM.RETURNCMD | TPM.NONOTIFY*/, x, y,
                ownerHandle, IntPtr.Zero);
            if (!result)
            {
                var hres = Marshal.GetHRForLastWin32Error();
                var ex = Marshal.GetExceptionForHR(hres);
                Debug.WriteLine("Failed to show menu. Handle is 0x{0:x}, error is: 0x{1:x}, hres: 0x{2:x}, exception: {3}", ownerHandle, Marshal.GetLastWin32Error(), hres, ex);
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    class MenuItemInfo {
        public Int32 cbSize = Marshal.SizeOf(typeof(MenuItemInfo));
        public MenuItemMask fMask;
        public MenuFlag fType;
        public MenuFlag fState;
        public UInt32 wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        public string? dwTypeData = null;
        public UInt32 cch; // length of dwTypeData
        public IntPtr hbmpItem;

        public MenuItemInfo() { }
        public MenuItemInfo(MenuItemMask pfMask) {
            fMask = pfMask;
        }
    }

    [Flags]
    enum TPM: uint
    {
        // Centers the shortcut menu horizontally relative to the coordinate specified by the x parameter.
        CENTERALIGN = 0x0004, 
        // Positions the shortcut menu so that its left side is aligned with the coordinate specified by the x parameter.
        LEFTALIGN = 0x0000, 
        // Positions the shortcut menu so that its right side is aligned with the coordinate specified by the x parameter.
        RIGHTALIGN = 0x0008, 
        // Positions the shortcut menu so that its bottom side is aligned with the coordinate specified by the y parameter.
        PM_BOTTOMALIGN = 0x0020, 
        // Positions the shortcut menu so that its top side is aligned with the coordinate specified by the y parameter.
        TOPALIGN = 0x0000, 
        // Centers the shortcut menu vertically relative to the coordinate specified by the y parameter.
        VCENTERALIGN = 0x0010, 
        // The function does not send notification messages when the user clicks a menu item.
        NONOTIFY = 0x0080, 
        // The function returns the menu item identifier of the user's selection in the return value.
        RETURNCMD = 0x0100, 
        // The user can select menu items with only the left mouse button.
        LEFTBUTTON = 0x0000, 
        // The user can select menu items with both the left and right mouse buttons.
        RIGHTBUTTON = 0x0002,
        // Animates the menu from right to left.
        HORNEGANIMATION = 0x0800, 
        // Animates the menu from left to right.
        HORPOSANIMATION = 0x0400, 
        // Displays menu without animation.
        NOANIMATION = 0x4000, 
        // Animates the menu from bottom to top.
        VERNEGANIMATION = 0x2000, 
        // Animates the menu from top to bottom.
        VERPOSANIMATION = 0x1000, 
    }

    [Flags]
    enum MenuFlag
    {
        // Uses a bitmap as the menu item. The lpNewItem parameter contains a handle to the bitmap.
        MF_BITMAP = 0x00000004,
        // Places a check mark next to the menu item. If the application provides check-mark bitmaps (see SetMenuItemBitmaps, this flag displays the check-mark bitmap next to the menu item.
        MF_CHECKED = 0x00000008,
        // Disables the menu item so that it cannot be selected, but the flag does not gray it.
        MF_DISABLED = 0x00000002,
        // Enables the menu item so that it can be selected, and restores it from its grayed state.
        MF_ENABLED = 0x00000000,
        // Disables the menu item and grays it so that it cannot be selected.
        MF_GRAYED = 0x00000001,
        // Functions the same as the MF_MENUBREAK flag for a menu bar. For a drop-down menu, submenu, or shortcut menu, the new column is separated from the old column by a vertical line.
        MF_MENUBARBREAK = 0x00000020,
        // Places the item on a new line (for a menu bar) or in a new column (for a drop-down menu, submenu, or shortcut menu) without separating columns.
        MF_MENUBREAK = 0x00000040,
        // Specifies that the item is an owner-drawn item. Before the menu is displayed for the first time, the window that owns the menu receives a WM_MEASUREITEM message to retrieve the width and height of the menu item. The WM_DRAWITEM message is then sent to the window procedure of the owner window whenever the appearance of the menu item must be updated.
        MF_OWNERDRAW = 0x00000100,
        // Specifies that the menu item opens a drop-down menu or submenu. The uIDNewItem parameter specifies a handle to the drop-down menu or submenu. This flag is used to add a menu name to a menu bar, or a menu item that opens a submenu to a drop-down menu, submenu, or shortcut menu.
        MF_POPUP = 0x00000010,
        // Draws a horizontal dividing line. This flag is used only in a drop-down menu, submenu, or shortcut menu. The line cannot be grayed, disabled, or highlighted. The lpNewItem and uIDNewItem parameters are ignored.
        MF_SEPARATOR = 0x00000800,
        // Specifies that the menu item is a text string; the lpNewItem parameter is a pointer to the string.
        MF_STRING = 0x00000000,
        // Does not place a check mark next to the item (default). If the application supplies check-mark bitmaps (see SetMenuItemBitmaps), this flag displays the clear bitmap next to the menu item.
        MF_UNCHECKED = 0x00000000,
    }

    [Flags]
    enum MenuItemMask
    {
        ///<summary> Retrieves or sets the hbmpItem member.</summary>
        MIIM_BITMAP = 0x00000080, 
        ///<summary> Retrieves or sets the hbmpChecked and hbmpUnchecked members.</summary>
        MIIM_CHECKMARKS = 0x00000008, 
        ///<summary> Retrieves or sets the dwItemData member.</summary>
        MIIM_DATA = 0x00000020, 
        ///<summary> Retrieves or sets the fType member.</summary>
        MIIM_FTYPE = 0x00000100, 
        ///<summary> Retrieves or sets the wID member.</summary>
        MIIM_ID = 0x00000002, 
        ///<summary> Retrieves or sets the fState member.</summary>
        MIIM_STATE = 0x00000001, 
        ///<summary> Retrieves or sets the dwTypeData member.</summary>
        MIIM_STRING = 0x00000040, 
        ///<summary> Retrieves or sets the hSubMenu member.</summary>
        MIIM_SUBMENU = 0x00000004, 
        ///<summary>
        ///Retrieves or sets the fType and dwTypeData members.
        /// MIIM_TYPE is replaced by MIIM_BITMAP, MIIM_FTYPE, and MIIM_STRING.</summary> 
        MIIM_TYPE = 0x00000010, 
     
    }
}