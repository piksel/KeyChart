using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Piksel.TextSymbols;
using ReactiveUI;

namespace KeyChart.GUI.Controls
{
    public class ActionBar : HeaderedItemsControl
    {
        public static readonly AttachedProperty<string> ActionProperty =
            AvaloniaProperty.RegisterAttached<ActionBar, Control, string>("Action");
        
        public static string GetAction(Control element) => element.GetValue(ActionProperty);
        public static void SetAction(Control element, string value) => element.SetValue(ActionProperty, value);
        
        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<ActionBar, string>(nameof(Label));
        
        public string Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly StyledProperty<TextSymbol?> SymbolProperty =
            AvaloniaProperty.Register<ActionBar, TextSymbol?>(nameof(Symbol));
        
        public TextSymbol? Symbol
        {
            get => GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        public Fa6 Fa6Symbol { set => Symbol = value; }

        public event EventHandler<string>? ButtonClicked;

        ReactiveCommand<string, Unit> ButtonClickedCommand { get; }

        static ActionBar()
        {
            // ItemsProperty.Changed.AddClassHandler<ActionBar>((t, a) => t.ItemsChanged(a));
        }        
        
        public ActionBar()
        {
            ButtonClickedCommand = ReactiveCommand.Create<string>(s => ButtonClicked?.Invoke(this, s));
        }

        protected override void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ItemsCollectionChanged(sender, e);
            if (e.NewItems != null)
            {
                foreach (Button b in e.NewItems.OfType<Button>())
                {
                    b.Click += (o, _) => ButtonClicked?.Invoke(o, b.GetValue(ActionProperty));
                    //b.Command = ButtonClickedCommand;
                }
            }
            
            // TODO: Remove event subscriptions?
            
        }
    }
}