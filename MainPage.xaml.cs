using System.Windows.Input;
using CommunityToolkit.Maui.Views;

namespace AudioLoopbackTest;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    private AudioLoopbackFFT _audioLoopbackFft;
    
    public MainPage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        BindingContext = this;
        
        _audioLoopbackFft = new AudioLoopbackFFT(serviceProvider);

        var spectrum = new SpectrumDrawable(serviceProvider);
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
            ((SpectrumDrawable)GraphicsView.Drawable).Data = args.Spectrum;
            GraphicsView.Invalidate();
        };
    }

    private async void OpenSettingsAsync(object sender, EventArgs e)
    {
        var settings = _serviceProvider.GetService<SettingsPopup>();
        await this.ShowPopupAsync(settings);
    }
}