using FftSharp;
using Complex = System.Numerics.Complex;

namespace AudioLoopbackTest;

public class SpectrumDrawable : IDrawable
{
    public Complex[] Data { get; set; }
    
    private int _bins;
    private float _binWidth;
    private double[] _heights;

    private AppTheme _theme = AppTheme.Light;
    
    public SpectrumDrawable(int bins)
    {
        _bins = bins;
        _heights = new double[bins];

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
        CalculateBinWidth(dirtyRect.Width);
        
        var magnitude = FFT.Magnitude(Data);
        for (int i = 0; i < _heights.Length; i++)
        {
            _heights[i] = GetYPos(magnitude[i], dirtyRect.Height);
        }

        for (int i = 0; i < _heights.Length; i++)
        {
            canvas.StrokeColor = Color.FromArgb(_theme == AppTheme.Dark ? "#FFF" : "#000");
            canvas.DrawRectangle(i*_binWidth, dirtyRect.Height, _binWidth, 200);
            // canvas.DrawRectangle(i*_binWidth, dirtyRect.Height, _binWidth, (float)_heights[i]*-1);
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

    private void CalculateBinWidth(double width)
    {
        _binWidth = (int)Math.Floor(width / _bins);
    }
}