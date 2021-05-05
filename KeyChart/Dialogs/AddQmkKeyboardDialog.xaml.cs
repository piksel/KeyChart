using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KeyChart.Keyboards;
using KeyChart.Keyboards.QMK;
using KeyChart.Pages;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
#nullable enable

namespace KeyChart.Dialogs
{
    public sealed partial class AddQmkKeyboardDialog : ContentDialog
    {
        public string[] KeyboardIDs = Array.Empty<string>();
        public ObservableCollection<string> KeyboardsFiltered = new();
        public ObservableCollection<string> Layouts = new();
        public ObservableCollection<KeyMap> KeyMaps = new();

        private string? selectedKeyboard;

        public static readonly DependencyProperty KeyboardInfoProperty = DependencyProperty.Register("KeyboardInfo", typeof(KeyboardInfo), typeof(AddQmkKeyboardDialog), null);

        public KeyboardInfo? KeyboardInfo
        {
            get => (KeyboardInfo)GetValue(KeyboardInfoProperty);
            set => SetValue(KeyboardInfoProperty, value);
        }

        public KeyMap? KeyMap => KeyMapPicker.SelectedItem as KeyMap;


        public AddQmkKeyboardDialog()
        {
            this.InitializeComponent();

            Loaded += async (_, __) => await TryOrSetError(UpdateKeyboardList, "Failed to fetch keyboards");

        }

        private async Task UpdateKeyboardList()
        {
            KeyboardSpinner.IsLoading = true;
            foreach (var path in await KeyboardHelper.ListKeyboards())
            {
                KeyboardsFiltered.Add(path);
            }
            KeyboardList.ItemsSource = KeyboardsFiltered;
            KeyboardSpinner.IsLoading = false;
        }

        private async Task TryOrSetError(Func<Task> action, string errorTitle = "An error occurred")
        {
            try
            {
                await action();
            }
            catch (Exception x)
            {
                await new MessageDialog($"{x.GetType().Name}: {x.Message}", errorTitle).ShowAsync();
            }
        }

        private async void ImportKeyMapButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new FileOpenPicker()
            {
                FileTypeFilter = { ".json" },
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            if (await picker.PickSingleFileAsync() is { } file)
            {
                await TryOrSetError(async () =>
                {
                    await KeyboardHelper.AddCustomKeyMap(file);

                });


            }
        }


        private async void LayoutPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsPrimaryButtonEnabled = false;
            if (LayoutPicker.SelectedItem is string layout)
            {
                await FetchKeyMaps(layout);
            }
        }

        private void KeyMapPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KeyMapPicker.SelectedItem is KeyMap keymap)
            {
                IsPrimaryButtonEnabled = true;
            }
        }

        private void KeyboardList_OnGotFocus(object sender, RoutedEventArgs e)
        {
            //KeyboardList.IsDropDownOpen = true;
        }

        private void KeyboardList_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            //KeyboardList.IsDropDownOpen = true;
        }

        private async void KeyboardList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedKeyboard = KeyboardList?.SelectedItem as string;

            await TryOrSetError(UpdateKeyboardInfo, "Failed to fetch keyboard");

        }

        private async Task UpdateKeyboardInfo()
        {
            if (selectedKeyboard is null || selectedKeyboard.Length < 1)
            {
                KeyboardInfo = null;
                return;
            }

            KeyMaps.Clear();
            KeyboardReadme.Text = String.Empty;


            KeyboardInfoSpinner.IsLoading = true;
            try
            {

                var keyboardInfo = await KeyboardHelper.GetKeyboardInfo(selectedKeyboard);
                KeyboardInfo = keyboardInfo;

                if (keyboardInfo != null)
                {
                    Layouts.Clear();

                    foreach (var layout in keyboardInfo.Layouts)
                    {
                        Layouts.Add(layout.Key);
                    }

                    var selectedLayout = keyboardInfo.Layouts?.Keys?.FirstOrDefault() ?? KeyboardHelper.DefaultLayout;
                    LayoutPicker.SelectedItem = selectedLayout;

                    // await FetchKeyboardReadme();
                }


                if (keyboardInfo == null) throw new Exception("Keyboard not found");
            }
            finally
            {
                KeyboardInfoPanel.Opacity = 1;
                KeyboardInfoSpinner.IsLoading = false;
            }
        }

        private async Task FetchKeyMaps(string layout)
        {
            if (selectedKeyboard == null) return;

            KeyMaps.Clear();


            await foreach (var keyMap in KeyboardHelper.ListKeyMaps(selectedKeyboard, layout))
            {
                KeyMaps.Add(keyMap);
            }

            //https://config.qmk.fm/keymaps/a/abacus_default.json
        }

        private async void PrimaryButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (KeyMap is { } keymap && KeyboardInfo is { } keyboardInfo)
            {
                var kbdModel = KeyboardModel.FromKeyboardKeyMap(keyboardInfo, keymap);
                await KeyboardHelper.AddKeyboardModel(kbdModel);
            }
        }
    }
}
