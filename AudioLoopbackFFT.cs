using System.Numerics;
using NAudio.Extras;
using NAudio.Wave;

namespace AudioLoopbackTest;

public partial class AudioLoopbackFFT{
    public event EventHandler<NewFftEventArgs> FFTCalculated;
    public int FFTBins { get; set; } = 512;
    
    public class NewFftEventArgs : EventArgs
    {
        public Complex[] Spectrum { get; }
        public NewFftEventArgs(Complex[] spectrum)
        {
            Spectrum = spectrum;
        }
    }
}