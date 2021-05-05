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
    public sealed partial class RenameDialog : ContentDialog
    {
        public static readonly DependencyProperty NewNameProperty = DependencyProperty.Register(
            "NewName", typeof(string), typeof(RenameDialog), new PropertyMetadata(default(string)));

        public string NewName
        {
            get => (string) GetValue(NewNameProperty);
            set => SetValue(NewNameProperty, value);
        }

        public static readonly DependencyProperty OldNameProperty = DependencyProperty.Register(
            "OldName", typeof(string), typeof(RenameDialog), new PropertyMetadata(default(string)));

        public string OldName
        {
            get => (string) GetValue(OldNameProperty);
            set => SetValue(OldNameProperty, value);
        }

        public RenameDialog()
        {
            InitializeComponent();

            RegisterPropertyChangedCallback(NewNameProperty, NewNameChanged);
        }

        private void NewNameChanged(DependencyObject sender, DependencyProperty dp)
        {
            var empty = string.IsNullOrEmpty(NewName);
            var unchanged = NewName == OldName;
            IsPrimaryButtonEnabled = !empty && !unchanged;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public static async Task<string> Show(string oldName, string title = "Rename")
        {
            var dialog = new RenameDialog
            {
                OldName = oldName,
                NewName = oldName,
                Title = title,
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary ? dialog.NewName : null;
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            NewNameTextbox.SelectAll();
        }
    }
}
