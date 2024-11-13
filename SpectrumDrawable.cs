using FftSharp;
using Complex = System.Numerics.Complex;
using Font = Microsoft.Maui.Graphics.Font;

namespace AudioLoopbackTest;

public class SpectrumDrawable : IDrawable
{
    private readonly IServiceProvider _serviceProvider;
    private float _contentWidth = 0;
    public float ContentWidth
    {
        get => _contentWidth;
        private set
        {
            if (value != _contentWidth)
            {
                float oldWidth = _contentWidth;
                _contentWidth = value;
                ContentWidthChanged?.Invoke(this, new ContentWidthChangedEventArgs(oldWidth, value));
            }
            else
            {
                _contentWidth = value;
            }
        }
    }

    public event EventHandler<ContentWidthChangedEventArgs> ContentWidthChanged;
    
    public Complex[] Data { get; set; }
    
    private double[] _heights;
    private double _windowWidth;

    private AppTheme _theme = AppTheme.Light;
    
    public SpectrumDrawable(IServiceProvider serviceProvider)
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

        if (settings.Normalize)
        {
            magnitude = Utils.Normalize(magnitude);
        }
        
        var binWidth = _windowWidth / magnitude.Length;
         
        if (binWidth < 1)
        {
            binWidth = 1;
        }
         
        if (_heights == null || _heights.Length != magnitude.Length)
        {
            _heights = new double[magnitude.Length];
        }
        
        for (int i = 0; i < _heights.Length; i++)
        {
            if (settings.Normalize)
            {
                _heights[i] = GetYPosNormalized(magnitude[i], dirtyRect.Height);
            }
            else
            {
                _heights[i] = GetYPos(magnitude[i], dirtyRect.Height);
            }
        }

        for (int i = 0; i < _heights.Length; i++)
        {
            canvas.StrokeColor = Color.FromArgb(_theme == AppTheme.Dark ? "#FFF" : "#000");
            // canvas.DrawRectangle(i*binWidth, dirtyRect.Height, binWidth, -200);
            canvas.DrawRectangle((float)(i*binWidth), dirtyRect.Height, (float)binWidth, (float)_heights[i]*-1);
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
    
    private double GetYPosLog(double intensityDB, float height)
    {
        // not entirely sure whether the multiplier should be 10 or 20 in this case.
        // going with 10 from here http://stackoverflow.com/a/10636698/7532
        // double intensityDB = 10 * Math.Log10(Math.Sqrt(c.Real * c.Real + c.Imaginary * c.Imaginary));
        double minDB = -90;
        if (intensityDB < minDB) intensityDB = minDB;
        double percent = intensityDB / minDB;
        // we want 0dB to be at the top (i.e. yPos = 0)
        double yPos = Math.Abs(percent * height);
        return yPos;
    }

    private double GetYPos(double magnitude, float height)
    {
        return magnitude * height * 80;
    }

    private double GetYPosNormalized(double magnitude, float height)
    {
        return magnitude * height;
    }
}

public class ContentWidthChangedEventArgs : EventArgs
{
    public float OldWidth { get; }
    public float NewWidth { get; }
    public ContentWidthChangedEventArgs(float oldWidth, float newWidth)
    {
        OldWidth = oldWidth;
        NewWidth = newWidth;
    }
}