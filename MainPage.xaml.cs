namespace AudioLoopbackTest;

public partial class MainPage : ContentPage
{

    private AudioLoopbackFFT _audioLoopbackFft;
    
    public MainPage()
    {
        InitializeComponent();
        
        _audioLoopbackFft = new AudioLoopbackFFT();
        
        GraphicsView.Drawable = new SpectrumDrawable(_audioLoopbackFft.FFTBins);

        _audioLoopbackFft.FFTCalculated += (sender, args) =>
        {
            ((SpectrumDrawable)GraphicsView.Drawable).Data = args.Spectrum;
            GraphicsView.Invalidate();
        };
    }
}