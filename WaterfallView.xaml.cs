using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using SkiaSharp;

namespace AudioLoopbackTest;

public partial class WaterfallView : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    private AudioLoopbackFFT _audioLoopbackFft;
    
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

        GraphicsView.Drawable = spectrum;
        GraphicsView.SizeChanged += (sender, args) =>
        {
            spectrum.WindowWidthChanged(GraphicsView.Width);
        };

        _audioLoopbackFft.FFTCalculated += (sender, args) =>
        {
            ((WaterfallDrawable)GraphicsView.Drawable).Data = args.Spectrum;
            GraphicsView.Invalidate();
        };
        
        
        SKCanvas canvasView = new SKCanvas();
    }

    private async void OpenSettingsAsync(object sender, EventArgs e)
    {
        var settings = _serviceProvider.GetService<SettingsPopup>();
        await this.ShowPopupAsync(settings);
    }
}