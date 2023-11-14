using FftSharp;
using NAudio.Wave;
using Complex = System.Numerics.Complex;

namespace AudioLoopbackTest;

public partial class AudioLoopbackFFT
{
    private WasapiLoopbackCapture _loopbackCapture;
    // private SampleAggregator _sampleAggregator;

    private Complex[] fftBuffer;

    private StreamWriter _audioData;
    
    public AudioLoopbackFFT()
    {
        // fftBuffer = new Complex[FFTBins];

        // _audioData = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        //     "audiolog.txt"));
        
        _loopbackCapture = new WasapiLoopbackCapture();

        _loopbackCapture.DataAvailable += (sender, args) =>
        {
            var dataBuffer = args.Buffer;
            int fftPos = 0;
            var audioBuffer = new double[args.Buffer.Length / 8];
            
            // foreach (var num in dataBuffer)
            // {
            //     _audioData.Write($"{num},");
            // }
            // _audioData.WriteLine();
            
            for (int i = 0; i < dataBuffer.Length; i += 4*2) // 2 channels at 4 bytes each (This should come from the device)
            {
               
                audioBuffer[i/8] = BitConverter.ToSingle(dataBuffer, i);
                // fftBuffer[fftPos].X = (float)(sample * FastFourierTransform.HammingWindow(fftPos, FFTBins));
                // fftBuffer[fftPos].Y = 0;
                // fftPos++;
                // if (fftPos >= fftBuffer.Length)
                // {
                //     FastFourierTransform.FFT(true, (int)Math.Log(FFTBins, 2.0), fftBuffer);
                //     FFTCalculated?.Invoke(this, new FftEventArgs(fftBuffer));
                //     break;
                // }
            }
            
            var window = new FftSharp.Windows.Hanning();
            window.ApplyInPlace(audioBuffer);

            var zeroPadded = FftSharp.Pad.ZeroPad(audioBuffer);
            Complex[] spectrum = FftSharp.FFT.Forward(zeroPadded);
            FFTCalculated?.Invoke(this, new NewFftEventArgs(spectrum));
        };
        
        _loopbackCapture.StartRecording();
        
    }
}