using System;
using System.Threading;
using System.Windows.Forms;


namespace KeyChart.HotkeyHelper
{
    internal class Program
    {
        internal static AppRPC Rpc = new AppRPC();
        private static void Main()
        {
            var mutexName = Rpc.MutexName;
            if (Mutex.TryOpenExisting(mutexName, out _)) return;

            var mutex = new Mutex(false, mutexName);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HotkeyAppContext());
            mutex.Close();
        }
    }
}
