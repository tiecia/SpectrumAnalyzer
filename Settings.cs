namespace AudioLoopbackTest;

public class Settings
{
    public int FftBins { get; set; } = 512;
    public bool BinFft { get; set; } = false;
    public bool Normalize { get; set; } = true;
    public bool NegativeFrequencies { get; set; } = false;
}