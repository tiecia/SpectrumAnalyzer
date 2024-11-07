using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;

namespace AudioLoopbackTest;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int SCREEN_CAPTURE_INTENT_CODE = 100;
    private AudioRecord _audioRecord;
    private Thread _thread;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _thread = new Thread(MonitorAudio);
        
        var manager = GetSystemService(MediaProjectionService) as MediaProjectionManager;
        StartActivityForResult(manager?.CreateScreenCaptureIntent(), SCREEN_CAPTURE_INTENT_CODE);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == SCREEN_CAPTURE_INTENT_CODE)
        {
            if (data != null)
            {
                var manager = GetSystemService(MediaProjectionService) as MediaProjectionManager;
                var mediaProjection = manager?.GetMediaProjection(-1, data);
                var config = new AudioPlaybackCaptureConfiguration.Builder(mediaProjection).Build();
                _audioRecord = new AudioRecord.Builder().SetAudioPlaybackCaptureConfig(config).Build();
                _thread.Start();
            }
        }
    }

    private void MonitorAudio()
    { 
        _audioRecord.StartRecording();
        var data = new byte[_audioRecord.BufferSizeInFrames];
        while (true)
        {
            var read = _audioRecord.Read(data, 0, data.Length);
        }
    }
}