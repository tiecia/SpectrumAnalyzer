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
        _settings = settings;
        InitializeComponent();
    }
    
    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void IncreaseBins(object sender, EventArgs e)
    {
        var currentBins = _settings.FFTBins;
        var nextPowerOfTwo = GetNextPowerOfTwo(currentBins);
        _settings.FFTBins = nextPowerOfTwo;
    }

    private void DecreaseBins(object sender, EventArgs e)
    {
        var currentBins = _settings.FFTBins;
        var priorPowerOfTwo = GetPriorPowerOfTwo(currentBins);
        _settings.FFTBins = priorPowerOfTwo;
    }
    
    public static int GetPriorPowerOfTwo(int number)
    {
        if (number < 1)
        {
            throw new ArgumentException("Number must be greater than 0.");
        }

        // Decrement number by 1 to handle exact powers of two correctly
        number |= (number >> 1);
        number |= (number >> 2);
        number |= (number >> 4);
        number |= (number >> 8);
        number |= (number >> 16);

        // Add 1 to the result and shift right by 1 to get the prior power of two
        return (number + 1) >> 1;
    }
    
    public static int GetNextPowerOfTwo(int number)
    {
        if (number < 1)
        {
            throw new ArgumentException("Number must be greater than 0.");
        }

        number--;

        number |= (number >> 1);
        number |= (number >> 2);
        number |= (number >> 4);
        number |= (number >> 8);
        number |= (number >> 16);

        return number + 1;
    }

    private void OnBinFFTChanged(object sender, CheckedChangedEventArgs e)
    {
        _settings.BinFFT = e.Value;
    }
}