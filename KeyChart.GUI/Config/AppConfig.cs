namespace KeyChart.GUI.Config
{
    public class AppConfig
    {
        public MainWindowConfig? MainWindow { get; set; }
        
        public record MainWindowConfig(int Width, int Height, int Left, int Top, int ConsoleHeight) 
            : WindowConfig(Width, Height, Left, Top);
    }
}