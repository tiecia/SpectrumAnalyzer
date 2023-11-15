using FftSharp;
using NAudio.Wave;
using Complex = System.Numerics.Complex;

namespace AudioLoopbackTest;

public partial class AudioLoopbackFFT
{
    private WasapiLoopbackCapture _loopbackCapture;

    private Complex[] fftBuffer;

    private StreamWriter _audioData;
    
    public AudioLoopbackFFT()
    {
        _loopbackCapture = new WasapiLoopbackCapture();

        _loopbackCapture.DataAvailable += (sender, args) =>
        {
            var dataBuffer = args.Buffer;
            int fftPos = 0;
            var audioBuffer = new double[args.Buffer.Length / 8];
            
            // TODO: Make compatible with all codecs (combine right channel?)
            for (int i = 0; i < dataBuffer.Length; i += 4*2) // 2 channels at 4 bytes each (This should come from the device)
            {
                audioBuffer[i/8] = BitConverter.ToSingle(dataBuffer, i);
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