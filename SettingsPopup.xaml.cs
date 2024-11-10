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
        BindingContext = settings;
        UpdateBins();
    }

    private void UpdateBins()
    {
        BinLabel.Text = $"Bins: {_settings.FftBins}";
        
        BinLabel.IsVisible = _settings.BinFft;
        IncreaseButton.IsEnabled = _settings.BinFft;
        DecreaseButton.IsEnabled = _settings.BinFft;
    }
    
    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void IncreaseBins(object sender, EventArgs e)
    {
        var currentBins = _settings.FftBins;
        var nextPowerOfTwo = NextPowerOfTwo(currentBins);
        _settings.FftBins = nextPowerOfTwo;

        UpdateBins();
    }

    private void DecreaseBins(object sender, EventArgs e)
    {
        var currentBins = _settings.FftBins;
        var priorPowerOfTwo = PrevPowerOfTwo(currentBins);
        if (priorPowerOfTwo < 2)
        {
            priorPowerOfTwo = 2;
        }
        _settings.FftBins = priorPowerOfTwo;
        
        UpdateBins();
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
        UpdateBins();
    }
}