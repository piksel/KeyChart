using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KeyChart.Dialogs;
using KeyChart.Keyboards.QMK;
using Piksel.UWP.Dialogs;
using Microsoft.Toolkit.Uwp.UI;
using muxc = Microsoft.UI.Xaml.Controls;
using KeyChart.Keyboards;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

#nullable enable

namespace KeyChart.Pages
{
    public class KeyboardPageDesignData
    {
        public RangeObservableCollection<string> KeyboardIDs { get; } = new(new []{ "Keyboard1", "Keyboard2" });
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class KeyboardPage : Page
    {

        public ObservableCollection<KeyboardModel> KeyboardModels = new();
        private string? selectedSource;


        public KeyboardPage()
        {
            this.InitializeComponent();

            if (DesignMode.DesignMode2Enabled)
            {

            }

            Loaded += async (_, __) => await TryOrSetError(UpdateKeyboardModels, "Failed to fetch keyboards");

        }





        private async Task TryOrSetError(Func<Task> action, string errorTitle = "An error occurred")
        {
            try
            {
                await action();
            }
            catch (Exception x)
            {
                ErrorBar.Title = errorTitle;
                ErrorBar.Content = $"{x.GetType().Name}: {x.Message}";
                ErrorBar.IsOpen = true;
            }
        }


        


        private async void AddQmkButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new AddQmkKeyboardDialog();
            if (await dialog.ShowAsync(ContentDialogPlacement.Popup) == ContentDialogResult.Primary)
            {
                await TryOrSetError(async () => await UpdateKeyboardModels());
            }
        }

        private async Task UpdateKeyboardModels()
        {
            selectedSource = KeyboardHelper.GetSelectedKeyboardModel();

            KeyboardModelsSpinner.IsLoading = true;
            try
            {

                KeyboardModels.Clear();
                KeyboardModel? selected = null;

                await foreach (var kbdModel in KeyboardHelper.ListKeyboardModels())
                {
                    KeyboardModels.Add(kbdModel);
                    if (kbdModel.Source == selectedSource)
                    {
                        selected = kbdModel;
                    }
                }

                if (selected is null) return;

                KeyboardModelPicker.SelectedItem = selected;
            }
            finally
            {
                KeyboardModelsSpinner.IsLoading = false;
            }
        }



        private async void AddFromFileButton_OnTapped(object sender, TappedRoutedEventArgs e)
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
                    using var stream = await file.OpenStreamForReadAsync();
                    var keymap = await QmkClient.ParseCustomKeymap(stream) ?? throw new Exception("Invalid keymap file");
                    var keyboardId = keymap.Keyboard ?? throw new Exception("Keymap has no keyboard specified");

                    var keyboardInfo = await KeyboardHelper.GetKeyboardInfo(keyboardId) ?? throw new Exception($"Keymap keyboard \"{keyboardId}\" could not be found");

                    var kbdModel = KeyboardModel.FromKeyboardKeyMap(keyboardInfo, keymap);
                    kbdModel.Name = file.DisplayName;

                    await KeyboardHelper.AddKeyboardModel(kbdModel);

                    await UpdateKeyboardModels();
                });
            }
        }

        private void Selected_Clicked(object sender, RoutedEventArgs e)
        {
            var kbdModel = (sender as FrameworkElement)?.DataContext as KeyboardModel;
            if (sender is AppBarToggleButton toggleButton)
            {
                if (GetModelContainer(kbdModel) is { } container)
                {
                    container.IsSelected = toggleButton.IsChecked ?? false;
                }
            }
        }

        private async void KeyboardCommand_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not AppBarButton {DataContext: KeyboardModel kbdModel} abb) return;

            switch (abb.Tag as string)
            {
                case "info":
                    await KeyboardInfoDialog.ShowDialog(kbdModel);
                    break;
                case "rename":
                    await RenameKeyboard(kbdModel);
                    break;
                case "delete":
                    await DeleteKeyboard(kbdModel);
                    break;
                case "rebuild":
                    await RebuildKeyboard(kbdModel);
                    break;
                default:
                    await new MessageDialog($"Not implemented: {abb.Tag}", "Action not implemented!").ShowAsync();
                    break;
            }
        }

        private async Task RenameKeyboard(KeyboardModel? kbdModel)
        {
            if (kbdModel is null) return;

            if (await RenameDialog.Show(kbdModel.Name, "Rename keyboard") is {} newName)
            {
                await KeyboardHelper.RenameKeyboard(kbdModel.Source, newName);
                await UpdateKeyboardModels();
            }
        }

        private async Task DeleteKeyboard(KeyboardModel? kbdModel)
        {
            if (kbdModel is null) return;

            if (await ConfirmDialog.Show($"Confirm deletion of keyboard \"{kbdModel.Name}\"?", "Delete keyboard"))
            {
                await KeyboardHelper.DeleteKeyboard(kbdModel.Source);
                await UpdateKeyboardModels();
            }
        }

        private async Task RebuildKeyboard(KeyboardModel? kbdModel)
        {
            if (kbdModel is null) return;

            await KeyboardHelper.RebuildKeyboard(kbdModel.Source);
            await UpdateKeyboardModels();
        }

        private ListViewItem? GetModelContainer(KeyboardModel? kbdModel)
            => KeyboardModelPicker.ContainerFromItem(kbdModel) as ListViewItem;

        private ListViewItem? GetMenuContainer(DependencyObject? c) =>
            c?.FindAscendantOrSelf<muxc.CommandBarFlyout>()?.Target?.FindAscendantOrSelf<ListViewItem>();

        private void KeyboardModelPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KeyboardModelPicker.SelectedItem is not KeyboardModel kbdModel) return;
            if (kbdModel.Source == selectedSource) return;
            
            KeyboardHelper.SetKeyboardModel(kbdModel);
            selectedSource = kbdModel.Source;
        }

        private void KeyboardMore_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is not AppBarButton abb) return;
            if (VisualTreeHelper.GetParent(abb) is not Grid grid) return;

            grid.ContextFlyout.ShowAt(grid);
        }

        private void KeyboardFlyout_OnOpening(object sender, object e)
        {
            if (sender is not muxc.CommandBarFlyout cbf) return;
            if (GetMenuContainer(cbf) is not { } container) return;
            if (cbf.SecondaryCommands.OfType<AppBarToggleButton>().FirstOrDefault() is not { } toggleButton) return;

            toggleButton.IsChecked = container.IsSelected;
        }


    }
}
