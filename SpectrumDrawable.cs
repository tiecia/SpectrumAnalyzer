using FftSharp;
using Complex = System.Numerics.Complex;

namespace AudioLoopbackTest;

public class SpectrumDrawable : IDrawable
{
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
    private float binWidth = 5;

    private AppTheme _theme = AppTheme.Light;
    
    public SpectrumDrawable()
    {
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
        var magnitude = FFT.Magnitude(Data);

        // var binWidth = CalculateBinWidth(dirtyRect.Width, magnitude.Length);
        ContentWidth = CalculateContentWidth(binWidth, magnitude.Length);

        if (_heights == null || _heights.Length != magnitude.Length)
        {
            _heights = new double[magnitude.Length];
        }
        
        for (int i = 0; i < _heights.Length; i++)
        {
            _heights[i] = GetYPos(magnitude[i], dirtyRect.Height);
        }

        for (int i = 0; i < _heights.Length; i++)
        {
            canvas.StrokeColor = Color.FromArgb(_theme == AppTheme.Dark ? "#FFF" : "#000");
            // canvas.DrawRectangle(i*binWidth, dirtyRect.Height, binWidth, -200);
            canvas.DrawRectangle(i*binWidth, dirtyRect.Height, binWidth, (float)_heights[i]*-1);
        }
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

    private double GetYPos(double magnitide, float height)
    {
        return magnitide * height * 80;
    }

    private float CalculateBinWidth(double width, int bins)
    {
        return 5;
        // _binWidth = (int)Math.Floor(width / bins);
    }

    private float CalculateContentWidth(float binWidth, int bins)
    {
        return 16384;
        // return binWidth * bins;
    }

    public void ZoomOut()
    {
        if (binWidth > 0)
        {
            binWidth--;
        }
    }

    public void ZoomIn()
    {
        if (binWidth < 100)
        {
            binWidth++;
        }
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