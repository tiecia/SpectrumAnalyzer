﻿using System.Windows.Input;
using CommunityToolkit.Maui.Views;

namespace AudioLoopbackTest;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    private AudioLoopbackFFT _audioLoopbackFft;
    
    public MainPage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        BindingContext = this;
        
        _audioLoopbackFft = new AudioLoopbackFFT(_serviceProvider);

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
    
    private async void OpenSettingsAsync(object sender, EventArgs e)
    {
        var settings = _serviceProvider.GetService<SettingsPopup>();
        await this.ShowPopupAsync(settings);
    }

    private void MenuItem_OnClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}