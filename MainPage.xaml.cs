using System.Timers;
using FftSharp;
using Complex = System.Numerics.Complex;
using Timer = System.Timers.Timer;

namespace AudioLoopbackTest;

public partial class MainPage : ContentPage
{

    private AudioLoopbackFFT _audioLoopbackFft;

    private DateTime _lastDraw = DateTime.Now;

    private StreamWriter _logFile;

    private Timer _refreshTimer;
    // WasapiLoopbackCapture loopbackCapture = new();

    // private SampleAggregator sampleAggregator;

    // private double[] Heights;
    

    // private int BinWidth;
    
    public MainPage()
    {
        InitializeComponent();

        Console.WriteLine("Init");

        _audioLoopbackFft = new AudioLoopbackFFT();

        // _logFile = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "fftlog.txt"));
        
        GraphicsView.Drawable = new SpectrumView(_audioLoopbackFft.FFTBins);

        // _refreshTimer = new Timer(20);
        // _refreshTimer.Elapsed += (Object source, ElapsedEventArgs args) =>
        // {
        //     try
        //     {
        //         GraphicsView.Invalidate();
        //     }
        //     catch (Exception e)
        //     {
        //         
        //     }
        // };
        // _refreshTimer.AutoReset = true;
        // _refreshTimer.Enabled = true;

        _audioLoopbackFft.FFTCalculated += (sender, args) =>
        {
            // foreach (var num in args.Spectrum)
            // {
            //     _logFile.Write($"{num.Real},");
            // }
            // _logFile.WriteLine();
            ((SpectrumView)GraphicsView.Drawable).Data = args.Spectrum;
            GraphicsView.Invalidate();
        };

        // sampleAggregator = new SampleAggregator(new SampleChannel(new WaveInProvider(loopbackCapture)), FftLength);
        //
        // sampleAggregator.FftCalculated += (s, a) =>
        // {
        //     // bool printed = false;
        //     // for (int i = 0; i < 10; i++)
        //     // {
        //     //     float result = a.Result[i].X*10000000;
        //     //     if (result != 0)
        //     //     {
        //     //         printed = true;
        //     //         Console.Write($"{result},");
        //     //     }
        //     // }
        //     //
        //     // if (printed)
        //     // {
        //     //     Console.WriteLine();
        //     // }
        //
        //     bool redraw = true;
        //     for (int i = 0; i < a.Result.Length; i++)
        //     {
        //         if (a.Result[i].X == 0)
        //         {
        //             redraw = false;
        //         }
        //     }
        //     
        //     if (redraw)
        //     {
        //         ((SpectrumView)GraphicsView.Drawable).Data = a.Result;
        //         GraphicsView.Invalidate();
        //     }
        //     else
        //     {
        //         Console.WriteLine("Wait");
        //     }
        //     
        // };
        //
        // sampleAggregator.PerformFFT = true;
        //
        // loopbackCapture.DataAvailable += (s, a) =>
        // {
        //     byte[] buffer = a.Buffer;
        //     int bytesRecorded = a.BytesRecorded;
        //     int bufferIncrement = loopbackCapture.WaveFormat.BlockAlign;
        //     float[] floatBuffer = new float[bytesRecorded];
        //
        //     int ind = 0;
        //     for (int index = 0; index < bytesRecorded; index += bufferIncrement)
        //     {
        //         float sample32 = BitConverter.ToSingle(buffer, index);
        //         floatBuffer[ind] = sample32;
        //         ind++;
        //     }
        //
        //     sampleAggregator.Read(floatBuffer, 0, bytesRecorded);
        // };
        //
        // loopbackCapture.RecordingStopped += (s, a) =>
        // {
        //     Console.WriteLine("Recording Stopped");
        // };
        //
        // loopbackCapture.StartRecording();
        //
        //
        // Console.WriteLine("Listening...");
    }
}

public class SpectrumView : IDrawable
{
    public Complex[] Data { get; set; }
    
    private int Bins;
    private float BinWidth;
    private double[] Heights;
    
    private const double MIN_BIN_HEIGHT = 10;

    private int _updateCount;
    
    private StreamWriter _logFile;
    
    public SpectrumView(int bins)
    {
        Bins = bins;
        Heights = new double[bins];
        
        // _logFile = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "datalog.txt"));

    }
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        CalculateBinWidth(dirtyRect.Width);
        
        // foreach (var num in Data)
        // {
        //     _logFile.Write($"{num.Real},");
        // }
        // _logFile.WriteLine();

        if (Data[0].Real == 0)
        {
            return;
        }

        if (Data == null)
        {
            for (int i = 0; i < Heights.Length; i++)
            {
                Heights[i] = MIN_BIN_HEIGHT;
            }
        }
        else
        {
            var magnitude = FFT.Magnitude(Data);
            for (int i = 0; i < Heights.Length; i++)
            {
                // Heights[i] = GetYPosLog(magnitude[i], dirtyRect.Height);
                Heights[i] = GetYPos(magnitude[i], dirtyRect.Height);
            }
        }

        for (int i = 0; i < Heights.Length; i++)
        {
            if(Heights[i] == 0) return;
            // canvas.DrawRectangle(i*BinWidth, 0, BinWidth, 100);
            canvas.DrawRectangle(i*BinWidth, dirtyRect.Height, BinWidth, (float)Heights[i]*-1);
        }
        // Thread.Sleep(20);
    }

    // public void UpdateBins(Complex[] data)
    // {
    //     Data = data;
    // }
    
    // public void CalculateBins(Complex[] data)
    // {
    //     for (int i = 0; i < data.Length; i++)
    //     {
    //         Heights[i] = GetYPosLog(data[i]);
    //     }
    //
    //     for (int i = 0; i < Bins; i++)
    //     {
    //         
    //     }
    // }
    
    private double GetYPosLog(double intensityDB, float height)
    {
        // not entirely sure whether the multiplier should be 10 or 20 in this case.
        // going with 10 from here http://stackoverflow.com/a/10636698/7532
        // double intensityDB = 10 * Math.Log10(Math.Sqrt(c.Real * c.Real + c.Imaginary * c.Imaginary));
        double minDB = -90;
        if (intensityDB < minDB) intensityDB = minDB;
        double percent = intensityDB / minDB;
        // we want 0dB to be at the top (i.e. yPos = 0)
        double yPos = Math.Abs(percent * height);
        return yPos;
    }

    private double GetYPos(double magnitide, float height)
    {
        // double percent = magnitide;
        return magnitide * height * 80;
    }

    private void CalculateBinWidth(double width)
    {
        BinWidth = (int)Math.Floor(width / Bins);
    }
}