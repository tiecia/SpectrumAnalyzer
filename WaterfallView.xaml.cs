using CommunityToolkit.Maui.Views;
using FftSharp;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Complex = System.Numerics.Complex;


namespace AudioLoopbackTest;

public partial class WaterfallView : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    private AudioLoopbackFFT _audioLoopbackFft;
    
    const string TEXT = "Hello, Bitmap!";
    private List<SKBitmap> _bitmaps = new List<SKBitmap>();

 
    private SKCanvasView _canvasView;
    private Complex[] _data;
    
    public WaterfallView(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        BindingContext = this;
        
        _audioLoopbackFft = new AudioLoopbackFFT(serviceProvider);

        var spectrum = new WaterfallDrawable(serviceProvider);
        // spectrum.ContentWidthChanged += (sender, args) =>
        // {
        //     GraphicsView.WidthRequest = args.NewWidth;
        // };

        // GraphicsView.Drawable = spectrum;
        // GraphicsView.SizeChanged += (sender, args) =>
        // {
        //     spectrum.WindowWidthChanged(GraphicsView.Width);
        // };

        _audioLoopbackFft.FFTCalculated += (sender, args) =>
        {
            // ((WaterfallDrawable)GraphicsView.Drawable).Data = args.Spectrum;
            // GraphicsView.Invalidate();
            _data = args.Spectrum;
            _canvasView.InvalidateSurface();
        };
        
        // using (SKPaint textPaint = new SKPaint { TextSize = 48 })
        // {
        //     SKRect bounds = new SKRect();
        //     textPaint.MeasureText(TEXT, ref bounds);
        //
        //     _bitmap = new SKBitmap((int)bounds.Right,
        //         (int)bounds.Height);
        //
        //     using (SKCanvas bitmapCanvas = new SKCanvas(_bitmap))
        //     {
        //         bitmapCanvas.Clear();
        //         bitmapCanvas.DrawText(TEXT, 0, -bounds.Top, textPaint);
        //     }
        // }
        
        _canvasView = new SKCanvasView();
        _canvasView.PaintSurface += CanvasViewOnPaintSurface;
        Content = _canvasView;
    }

    private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (_data is null)
        {
            return;
        }
        
        SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        
        var settings = _serviceProvider.GetService<Settings>();
        
        var magnitude = FFT.Magnitude(_data, positiveOnly:!settings.NegativeFrequencies);

        if (settings.Normalize)
        {
            magnitude = Utils.Normalize(magnitude);
        }
        
        var colors = new SKColor[magnitude.Length];
        
        for (int i = 0; i < magnitude.Length; i++)
        {
            var color = Utils.MapToRange(magnitude[i], 0, 1, 0, 255);
            colors[i] = new SKColor((byte)color, 0, 0);
            // colors[i] = SKColor.FromHsv(0, 1, (float)color);
        }

        var freqHeight = info.Height / magnitude.Length;
        
        if (freqHeight == 0)
        {
            freqHeight = 1;
        }
        
        canvas.Clear(SKColors.Black);
        
        var bitmap = new SKBitmap(5, info.Height);
        
        var bound = freqHeight * magnitude.Length;
        
        if (bitmap.Height < bound)
        {
            bound = bitmap.Height;
        }
        
        // Set all pixels to blue
        for (int y = 0; y < bound; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                bitmap.SetPixel(x, y, colors[y / freqHeight]);
            }
        }
        
        _bitmaps.Insert(0, bitmap);

        var width = 0;
        foreach (var bm in _bitmaps)
        {
            canvas.DrawBitmap(bm, new SKPoint(width, 0));
            width += bm.Width;
        }
        
        canvas.Flush();

        var maxBitmaps = info.Width / bitmap.Width;
        _bitmaps.RemoveAll(bm => _bitmaps.IndexOf(bm) >= maxBitmaps);
    }

    private async void OpenSettingsAsync(object sender, EventArgs e)
    {
        var settings = _serviceProvider.GetService<SettingsPopup>();
        await this.ShowPopupAsync(settings);
    }
}