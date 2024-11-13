using FftSharp;
using Complex = System.Numerics.Complex;
using Font = Microsoft.Maui.Graphics.Font;

namespace AudioLoopbackTest;

public class WaterfallDrawable : IDrawable
{
    private readonly IServiceProvider _serviceProvider;

    public Complex[] Data { get; set; }
    
    private double[] _heights;
    private double _windowWidth;

    private AppTheme _theme = AppTheme.Light;
    
    public WaterfallDrawable(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                _theme = a.RequestedTheme;
            };
        }
    }
    public void Draw(ICanvas canvas, RectF dirtyRect)
    { 
        if (Data == null)
        {
            return;
        }
        
        var settings = _serviceProvider.GetService<Settings>();
        
        var magnitude = FFT.Magnitude(Data, positiveOnly:!settings.NegativeFrequencies);
        
        for (int x = 0; x < dirtyRect.Width; x++)
        {
            // for (int y = 0; y < dirtyRect.Height; y++)
            // {
            //     var color = Color.FromRgb(255, 0, 0);
            //     canvas.FillColor = color;
            //     canvas.FillRectangle(x, y, 1, 1);
            // }
            // canvas.FillRectangle(x, 0, 2, dirtyRect.Height);
        }

        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 30;
        canvas.DrawString("Frequency (Hz)", dirtyRect.Width - 250, dirtyRect.Height-70, 250, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
        canvas.DrawString("Relative Power (RMS)", 15, 15, 350, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
    }
    
    public void WindowWidthChanged(double width)
    {
        _windowWidth = width;
    }
}