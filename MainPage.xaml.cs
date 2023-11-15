using System.Windows.Input;

namespace AudioLoopbackTest;

public partial class MainPage : ContentPage
{

    private AudioLoopbackFFT _audioLoopbackFft;
    
    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        
        _audioLoopbackFft = new AudioLoopbackFFT();

        var spectrum = new SpectrumDrawable();
        spectrum.ContentWidthChanged += (sender, args) =>
        {
            GraphicsView.WidthRequest = args.NewWidth;
        };
        
        GraphicsView.Drawable = spectrum;

        _audioLoopbackFft.FFTCalculated += (sender, args) =>
        {
            ((SpectrumDrawable)GraphicsView.Drawable).Data = args.Spectrum;
            GraphicsView.Invalidate();
        };
    }

    private void ZoomInClicked(object sender, EventArgs e)
    {
        ((SpectrumDrawable)GraphicsView.Drawable).ZoomIn();
    }

    private void ZoomOutClicked(object sender, EventArgs e)
    {
        ((SpectrumDrawable)GraphicsView.Drawable).ZoomOut();
    }
}