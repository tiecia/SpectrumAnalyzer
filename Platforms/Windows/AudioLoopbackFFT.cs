using FftSharp;
using NAudio.Wave;
using Complex = System.Numerics.Complex;

namespace AudioLoopbackTest;

public partial class AudioLoopbackFFT
{
    private readonly WasapiLoopbackCapture _loopbackCapture;

    public AudioLoopbackFFT(IServiceProvider serviceProvider)
    {
        _loopbackCapture = new WasapiLoopbackCapture();

        var captureDevice = WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice();
        
        _loopbackCapture.DataAvailable += (sender, args) =>
        {
            var settings = serviceProvider.GetService<Settings>();
            
            var dataBuffer = args.Buffer;
            var audioBuffer = new double[args.Buffer.Length / 8];
            
            // TODO: Make compatible with all codecs (combine right channel?)
            for (int i = 0; i < dataBuffer.Length; i += 4*2) // 2 channels at 4 bytes each (This should come from the device)
            {
                audioBuffer[i/8] = BitConverter.ToSingle(dataBuffer, i);
            }
            
            var binFft = settings.BinFFT;
            
            double[] fftBuffer;
            if (binFft)
            {
                var fftBins = settings.FFTBins;
                
                var combineSize = audioBuffer.Length / fftBins;
                fftBuffer = new double[fftBins];
                for (int i = 0; i < fftBins; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < combineSize; j++)
                    {
                        sum += audioBuffer[i * combineSize + j];
                    }
                    fftBuffer[i] = sum / combineSize;
                }
            }
            else
            {
                fftBuffer = audioBuffer;
            }
            
            var window = new FftSharp.Windows.Hanning();
            window.ApplyInPlace(audioBuffer);

            var zeroPadded = FftSharp.Pad.ZeroPad(fftBuffer);
            Complex[] spectrum = FftSharp.FFT.Forward(zeroPadded);
            FFTCalculated?.Invoke(this, new NewFftEventArgs(spectrum));
        };
        
        _loopbackCapture.StartRecording();
        
    }
}