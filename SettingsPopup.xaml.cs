using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;

namespace AudioLoopbackTest;

public partial class SettingsPopup : Popup
{
    private readonly Settings _settings;

    public SettingsPopup(Settings settings)
    {
        InitializeComponent();
        
        _settings = settings;
        BinFftCheckbox.IsChecked = settings.BinFFT;
    }
    
    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void IncreaseBins(object sender, EventArgs e)
    {
        var currentBins = _settings.FFTBins;
        var nextPowerOfTwo = NextPowerOfTwo(currentBins);
        _settings.FFTBins = nextPowerOfTwo;
    }

    private void DecreaseBins(object sender, EventArgs e)
    {
        var currentBins = _settings.FFTBins;
        var priorPowerOfTwo = PrevPowerOfTwo(currentBins);
        if (priorPowerOfTwo < 2)
        {
            priorPowerOfTwo = 2;
        }
        _settings.FFTBins = priorPowerOfTwo;
    }
    
    public static int NextPowerOfTwo(int num)
    {
        var currentPower = Math.Log2(num);
        var nextPower = currentPower + 1;
        return (int)Math.Pow(2, nextPower);
    } 
    
    public static int PrevPowerOfTwo(int num)
    {
        var currentPower = Math.Log2(num);
        var nextPower = currentPower - 1;
        return (int)Math.Pow(2, nextPower);
    } 
    
    private void OnBinFFTChanged(object sender, CheckedChangedEventArgs e)
    {
        _settings.BinFFT = e.Value;
    }
}