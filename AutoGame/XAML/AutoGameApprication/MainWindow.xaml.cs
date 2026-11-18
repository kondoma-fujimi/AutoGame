using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Window = System.Windows.Window;

namespace AutoGameApprication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ControlWindow _controlWindow;
        private readonly DispatcherTimer _captureTimer;
        private VideoCapture? _capture;
        private Mat? _currentFrame;
        private bool _isRecording;
        private int _recordingFrameIndex;

        public bool IsCapturing => _capture?.IsOpened() == true;
        public bool IsRecording => _isRecording;
        public GameSettings Settings { get; set; }
        public string OutputDirectory
        {
            get => Settings.OutputDirectory;
            set => Settings.OutputDirectory = value;
        }

        public MainWindow(GameSettings? settings = null)
        {
            Settings = settings ?? new GameSettings();
            InitializeComponent();
            _captureTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33)
            };
            _captureTimer.Tick += CaptureTimer_Tick;
            // ControlWindow を先に生成するが、Owner の設定と Show は
            // ウィンドウが表示された後（Loaded イベント）に行う
            _controlWindow = new ControlWindow(this);
            Loaded += MainWindow_Loaded;
            Closed += (_, _) => StopCapture();
            AppLog.Write($"ゲーム画面を開始しました: {Settings.GameTitle}");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 一度だけ実行する
            Loaded -= MainWindow_Loaded;
            _controlWindow.Owner = this;
            _controlWindow.Show();
            _controlWindow.ShowLogWindow();
        }

        public void ToggleCapture()
        {
            if (IsCapturing)
            {
                StopCapture();
                return;
            }

            int deviceIndex = Settings.CaptureDeviceIndex;
            var capture = new VideoCapture(deviceIndex, VideoCaptureAPIs.DSHOW);
            if (!capture.IsOpened())
            {
                capture.Dispose();
                throw new InvalidOperationException($"キャプチャデバイス番号{deviceIndex}を開けませんでした。接続と選択したデバイス番号を確認してください。");
            }

            capture.Set(VideoCaptureProperties.FrameWidth, 1920);
            capture.Set(VideoCaptureProperties.FrameHeight, 1080);
            _capture = capture;
            _captureTimer.Start();
            AppLog.Write($"キャプチャデバイス番号{deviceIndex}に接続しました。");
        }

        public void SaveScreenshot()
        {
            if (_currentFrame is null || _currentFrame.Empty())
            {
                throw new InvalidOperationException("保存できる映像フレームがありません。");
            }

            string directory = Path.Combine(OutputDirectory, "image");
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            if (!Cv2.ImWrite(path, _currentFrame))
            {
                throw new IOException($"スクリーンショットを保存できませんでした: {path}");
            }

            MessageBox.Show(this, $"保存しました。\n{path}", "撮影", MessageBoxButton.OK, MessageBoxImage.Information);
            AppLog.Write($"スクリーンショットを保存しました: {path}");
        }

        public void ToggleRecording()
        {
            if (_isRecording)
            {
                _isRecording = false;
                return;
            }

            if (!IsCapturing)
            {
                throw new InvalidOperationException("録画を開始するには、先にキャプチャボードへ接続してください。");
            }

            Directory.CreateDirectory(Path.Combine(OutputDirectory, "frames"));
            _recordingFrameIndex = 0;
            _isRecording = true;
            AppLog.Write("録画を開始しました。");
        }

        private void CaptureTimer_Tick(object? sender, EventArgs e)
        {
            if (_capture is null)
            {
                return;
            }

            using var frame = new Mat();
            if (!_capture.Read(frame) || frame.Empty())
            {
                return;
            }

            _currentFrame?.Dispose();
            _currentFrame = frame.Clone();
            GameScreen.Source = BitmapSourceConverter.ToBitmapSource(frame);

            if (_isRecording)
            {
                string path = Path.Combine(
                    OutputDirectory,
                    "frames",
                    $"frame_{_recordingFrameIndex++:D5}.png");
                Cv2.ImWrite(path, frame);
            }
        }

        private void StopCapture()
        {
            bool wasCapturing = _capture is not null;
            bool wasRecording = _isRecording;
            _captureTimer.Stop();
            _isRecording = false;
            _capture?.Release();
            _capture?.Dispose();
            _capture = null;
            if (wasRecording)
            {
                AppLog.Write("アプリ終了時に録画を停止しました。");
            }
            if (wasCapturing)
            {
                AppLog.Write("キャプチャボードの接続を終了しました。");
            }
            _currentFrame?.Dispose();
            _currentFrame = null;
            GameScreen.Source = null;
        }
    }
}