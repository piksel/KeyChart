using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace KeyChart.GUI.Controls
{
    public class LoadingIndicator : TemplatedControl
    {
        private string _message = "Loading...";
        
        public static readonly DirectProperty<LoadingIndicator, string> MessageProperty =
            AvaloniaProperty.RegisterDirect<LoadingIndicator, string>(
                nameof(Message),
                o => o.Message);

        public string Message
        {
            get => _message;
            set => SetAndRaise(MessageProperty, ref _message, value);
        }
        
        private Color _secondaryColor = Color.FromArgb(0, 255, 0, 255);
        
        public static readonly DirectProperty<LoadingIndicator, Color> SecondaryColorProperty =
            AvaloniaProperty.RegisterDirect<LoadingIndicator, Color>(
                nameof(SecondaryColor),
                o => o.SecondaryColor);

        public Color SecondaryColor
        {
            get => _secondaryColor;
            set
            {
                SetAndRaise(SecondaryColorProperty, ref _secondaryColor,  new Color(0, value.R, value.G, value.B));
                CircleBrush = CreateCircleBrush(PrimaryColor, _secondaryColor);

            }
        }
        
        private Color _primaryColor = Color.FromArgb(0, 128, 0, 255);
        
        public static readonly DirectProperty<LoadingIndicator, Color> PrimaryColorProperty =
            AvaloniaProperty.RegisterDirect<LoadingIndicator, Color>(
                nameof(PrimaryColor),
                o => o.PrimaryColor);

        public Color PrimaryColor
        {
            get => _primaryColor;
            set
            {
                SetAndRaise(PrimaryColorProperty, ref _primaryColor, value);
                CircleBrush = CreateCircleBrush(value, SecondaryColor);
            }
        }

        private IConicGradientBrush _circleBrush = CreateCircleBrush(Colors.White, Colors.Transparent);

        private static IConicGradientBrush CreateCircleBrush(Color primary, Color secondary)
            => new ConicGradientBrush
            {
                GradientStops = new GradientStops
                {
                    new() {Color = secondary, Offset = 0},
                    new() {Color = secondary, Offset = .1},
                    new() {Color = primary, Offset = 1},
                },
            };

        public static readonly DirectProperty<LoadingIndicator, IConicGradientBrush> CircleBrushProperty =
            AvaloniaProperty.RegisterDirect<LoadingIndicator, IConicGradientBrush>(
                nameof(CircleBrush),
                o => o.CircleBrush);

        public IConicGradientBrush CircleBrush
        {
            get => _circleBrush;
            private set => SetAndRaise(CircleBrushProperty, ref _circleBrush, value);
        }

    }
}