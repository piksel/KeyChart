using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238


// #nullable enable

namespace Piksel.UWP.Dialogs
{
    public sealed partial class ConfirmDialog : ContentDialog
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(RenameDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get => (string) GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }


        public ConfirmDialog()
        {
            InitializeComponent();
        }

        public static async Task<bool> Show(string message, string title = "Rename")
        {
            var dialog = new ConfirmDialog
            {
                Message = message,
                Title = title,
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }

    }
}
