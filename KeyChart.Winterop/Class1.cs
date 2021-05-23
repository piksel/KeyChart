using System;

namespace Microsoft.Windows.Sdk
{
    public class Class1
    {
        static void Test() {
            PInvoke.RegisterHotKey((HWND) 0, (int) 0, (RegisterHotKey_fsModifiersFlags) (int) 0, (uint) 0);
        }
    }
}