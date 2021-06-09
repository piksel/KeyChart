using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KeyChart.Keyboards;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
#nullable enable
namespace KeyChart.Pages
{
    public sealed partial class ChartPage : Page
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(LayerCollection), typeof(ChartPage), null);

        public KeyboardModel? Model
        {
            get => (KeyboardModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public static readonly DependencyProperty NoKeyboardProperty = DependencyProperty.Register(
            "NoKeyboard", typeof(bool), typeof(ChartPage), new PropertyMetadata(default(bool)));

        public bool NoKeyboard
        {
            get => (bool) GetValue(NoKeyboardProperty);
            set => SetValue(NoKeyboardProperty, value);
        }

        public ChartPage()
        {
            this.InitializeComponent();


            //RegisterPropertyChangedCallback(KeyMapProperty, UpdateKeyMap);
            //RegisterPropertyChangedCallback(ModelProperty, (_, __) => UpdateKeyLayout());
            Loaded += async (_, __) =>
            {
                var model = await KeyboardHelper.GetKeyboardModel();
                if (model is {} && KeyboardHelper.KeyboardFailedToLoad() == model.Source)
                {
                    //KeyboardHelper.ClearSelectedKeyboardModel();
                    //model = null;
                }

                if (model == null)
                {
                    NoKeyboard = true;
                }
                else
                {
                    KeyboardHelper.MarkBeginLoadingKeyboard(model.Source);
                    Model = model;
                    // TODO: Fix the reset, or remove this all together!
                    KeyboardHelper.MarkLoadingKeyboardDone();
                    // Model.BuildKeyLayout();
                    //UpdateKeyLayout();
                }
                
                Spinner.IsLoading = false;
            };

            App.Current.UnhandledException += (sender, ea) =>
            {
                ea.Handled = true;
                KeyboardHelper.ClearSelectedKeyboardModel();
            };
        }

        private void UpdateKeyLayout()
        {
            KeyContainer.Items.Clear();
            if (Model is null) return;

            foreach (var key in Model.Layout)
            {
                var lk = new LayerKey()
                {
                    Key = key,
                    // Margin = key.Margin,
                };
                KeyContainer.Items.Add(lk);
            }

            KeyContainer.HorizontalAlignment = HorizontalAlignment.Center;
            KeyContainer.VerticalAlignment = VerticalAlignment.Center;

        }

        private void Layout_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void UpdateKeyMap(DependencyObject sender, DependencyProperty dp)
        {
            
        }

        private void KeyboardSettingsLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
    }
}
