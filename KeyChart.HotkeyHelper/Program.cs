using System;
using System.Threading;
using System.Windows.Forms;
using Windows.ApplicationModel;

namespace KeyChart.HotkeyHelper
{
    internal class Program
    {
        private static void Main()
        {
            var mutexName = Package.Current.Id.FamilyName + "_HotkeyHelperMutex";
            if (Mutex.TryOpenExisting(mutexName, out _)) return;

            var mutex = new Mutex(false, mutexName);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HotkeyAppContext());
            mutex.Close();
        }
    }
}
