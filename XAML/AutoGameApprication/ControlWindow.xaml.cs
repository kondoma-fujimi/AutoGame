using Microsoft.Win32;
using OpenCvSharp;
using System.IO;
using System.IO.Ports;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Window = System.Windows.Window;

namespace AutoGameApprication
{
    public partial class ControlWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private SwitchController? _switchController;
        private AutoGameWindow? _autoGameWindow;
        private LogWindow? _logWindow;
        private string? _settingsFilePath;
        private bool _updatingComPorts;
        private bool _updatingCaptureDevices;

        public ControlWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            GameTitleLabel.Text = _mainWindow.Settings.GameTitle;
            RefreshComPorts(_mainWindow.Settings.ComPortName);
            RefreshCaptureDevices(_mainWindow.Settings.CaptureDeviceIndex);
            Closed += (_, _) => StopManualControl();
            AppLog.Write($"設定を読み込みました: {_mainWindow.Settings.GameTitle}");
        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAutoGameWindow();
        }

        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleManualControl();
                StatusText.Text = _switchController is null
                    ? "Manual操作を終了しました"
                    : $"{_mainWindow.Settings.ComPortName}を開いて送信中（受信確認なし）";
            }
            catch (Exception exception)
            {
                AppLog.Write($"Manual操作エラー: {exception.Message}");
                StatusText.Text = $"Manual接続エラー: {exception.Message}";
                ShowOperationError(exception);
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindow.ToggleCapture();
                StatusText.Text = _mainWindow.IsCapturing
                    ? "キャプチャボードに接続しました"
                    : "キャプチャボードの接続を終了しました";
            }
            catch (Exception exception)
            {
                AppLog.Write($"キャプチャボード接続エラー: {exception.Message}");
                ShowOperationError(exception);
            }
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindow.SaveScreenshot();
                StatusText.Text = "スクリーンショットを保存しました";
            }
            catch (Exception exception)
            {
                AppLog.Write($"撮影エラー: {exception.Message}");
                ShowOperationError(exception);
            }
        }

        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool wasRecording = _mainWindow.IsRecording;
                _mainWindow.ToggleRecording();
                StatusText.Text = _mainWindow.IsRecording ? "録画中" : "録画を停止しました";
                if (wasRecording && !_mainWindow.IsRecording)
                {
                    AppLog.Write("録画を停止しました。");
                    MessageBox.Show(this, "録画を停止しました。", "録画", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception exception)
            {
                AppLog.Write($"録画エラー: {exception.Message}");
                ShowOperationError(exception);
            }
        }

        private void OpenSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "設定ファイルを開く",
                Filter = "AutoGame設定 (*.json)|*.json|すべてのファイル (*.*)|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(dialog.FileName);
                GameSettings? settings = JsonSerializer.Deserialize<GameSettings>(json);
                if (settings is null ||
                    string.IsNullOrWhiteSpace(settings.GameTitle) ||
                    string.IsNullOrWhiteSpace(settings.OutputDirectory) ||
                    string.IsNullOrWhiteSpace(settings.ComPortName) ||
                    settings.CaptureDeviceIndex < 0)
                {
                    throw new InvalidDataException("設定ファイルにゲームタイトル、出力先、COMポート、有効なキャプチャデバイス番号を指定してください。");
                }

                if (_switchController is not null)
                {
                    StopManualControl();
                }

                _mainWindow.Settings = settings;
                _settingsFilePath = dialog.FileName;
                GameTitleLabel.Text = settings.GameTitle;
                RefreshComPorts(settings.ComPortName);
                RefreshCaptureDevices(settings.CaptureDeviceIndex);
                AppLog.Write($"設定ファイルを開きました: {dialog.FileName} ({settings.GameTitle})");
                StatusText.Text = $"設定を読み込みました: {settings.GameTitle}";
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException or InvalidDataException)
            {
                AppLog.Write($"設定ファイルを開けませんでした: {exception.Message}");
                ShowOperationError(exception);
            }
        }

        private void SaveSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_settingsFilePath))
            {
                SaveSettingsAsMenuItem_Click(sender, e);
                return;
            }

            SaveSettingsToFile(_settingsFilePath);
        }

        private void SaveSettingsAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "設定ファイルを保存",
                Filter = "AutoGame設定 (*.json)|*.json",
                DefaultExt = ".json",
                AddExtension = true,
                FileName = $"{MakeSafeFileName(_mainWindow.Settings.GameTitle)}.json",
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(this) == true)
            {
                SaveSettingsToFile(dialog.FileName);
            }
        }

        private void SaveSettingsToFile(string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    _mainWindow.Settings,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                _settingsFilePath = filePath;
                AppLog.Write($"設定ファイルを保存しました: {filePath}");
                StatusText.Text = $"設定を保存しました: {Path.GetFileName(filePath)}";
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AppLog.Write($"設定ファイルを保存できませんでした: {exception.Message}");
                ShowOperationError(exception);
            }
        }

        private void ShowLogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowLogWindow();
        }

        internal void ShowLogWindow()
        {
            if (_logWindow is null)
            {
                _logWindow = new LogWindow
                {
                    Owner = _mainWindow
                };
                _logWindow.Closed += (_, _) => _logWindow = null;
            }

            if (!_logWindow.IsVisible)
            {
                _logWindow.Show();
            }
            else
            {
                _logWindow.Activate();
            }
        }

        private void ComPortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_updatingComPorts || ComPortComboBox.SelectedItem is not string portName)
            {
                return;
            }

            if (_switchController is not null)
            {
                StopManualControl();
                StatusText.Text = "COMポート変更のためManual操作を終了しました";
            }

            _mainWindow.Settings.ComPortName = portName;
            AppLog.Write($"使用COMポートを変更しました: {portName} (115200 bps)");
        }

        private void RefreshComPortsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshComPorts(_mainWindow.Settings.ComPortName);
            StatusText.Text = ComPortComboBox.Items.Count == 0
                ? "COMポートが見つかりません。接続を確認して再取得してください"
                : $"COMポート候補を再取得しました（{ComPortComboBox.Items.Count}件）";
        }

        private void CaptureDeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_updatingCaptureDevices || CaptureDeviceComboBox.SelectedItem is not int deviceIndex)
            {
                return;
            }

            _mainWindow.Settings.CaptureDeviceIndex = deviceIndex;
            AppLog.Write($"キャプチャデバイス番号を変更しました: {deviceIndex}");
        }

        private void RefreshComPorts(string selectedPort)
        {
            _updatingComPorts = true;
            try
            {
                ComPortComboBox.Items.Clear();
                string[] detectedPorts = SerialPort.GetPortNames()
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                foreach (string portName in detectedPorts)
                {
                    ComPortComboBox.Items.Add(portName);
                }

                ComPortComboBox.SelectedItem = detectedPorts.FirstOrDefault(
                    portName => string.Equals(portName, selectedPort, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _updatingComPorts = false;
            }

            ComPortInfoText.Text = ComPortComboBox.Items.Count == 0
                ? "COMポート未検出"
                : "115200 bps / 8N1";
        }

        private void RefreshCaptureDevices(int selectedDeviceIndex)
        {
            _updatingCaptureDevices = true;
            try
            {
                CaptureDeviceComboBox.Items.Clear();
                for (int deviceIndex = 0; deviceIndex <= 9; deviceIndex++)
                {
                    try
                    {
                        using var capture = new VideoCapture(deviceIndex, VideoCaptureAPIs.DSHOW);
                        if (capture.IsOpened())
                        {
                            CaptureDeviceComboBox.Items.Add(deviceIndex);
                        }
                    }
                    catch (OpenCvSharpException exception)
                    {
                        AppLog.Write($"キャプチャデバイス番号{deviceIndex}を確認できませんでした: {exception.Message}");
                    }
                }

                if (!CaptureDeviceComboBox.Items.Contains(selectedDeviceIndex))
                {
                    CaptureDeviceComboBox.Items.Add(selectedDeviceIndex);
                }

                CaptureDeviceComboBox.SelectedItem = selectedDeviceIndex;
            }
            finally
            {
                _updatingCaptureDevices = false;
            }
        }

        private void ToggleManualControl()
        {
            if (_switchController is not null)
            {
                StopManualControl();
                return;
            }

            if (ComPortComboBox.SelectedItem is not string portName)
            {
                throw new InvalidOperationException(
                    $"使用可能なCOMポートが選択されていません。現在の設定は{_mainWindow.Settings.ComPortName}です。機器を接続して「再取得」を押し、検出されたポートを選択してください。");
            }

            var controller = new SwitchController(portName);
            controller.Disconnected += (_, _) =>
            {
                if (ReferenceEquals(_switchController, controller))
                {
                    _switchController = null;
                    StatusText.Text = "Manualコントローラーが切断されました";
                    AppLog.Write("Manualコントローラーが切断されました。");
                }
            };
            controller.Start();
            _switchController = controller;
            AppLog.Write($"{portName}を開き、115200bps / 8N1でManualデータ送信を開始しました。");
        }

        private void StopManualControl()
        {
            _switchController?.Dispose();
            if (_switchController is not null)
            {
                AppLog.Write("Manual操作を終了しました。");
            }
            _switchController = null;
        }

        private void ShowAutoGameWindow()
        {
            if (_autoGameWindow is null)
            {
                _autoGameWindow = new AutoGameWindow
                {
                    Owner = _mainWindow
                };
                _autoGameWindow.Closed += (_, _) => _autoGameWindow = null;
            }

            if (!_autoGameWindow.IsVisible)
            {
                _autoGameWindow.Show();
            }
            else
            {
                _autoGameWindow.Activate();
            }
        }

        private void ShowOperationError(Exception exception)
        {
            MessageBox.Show(this, exception.Message, "操作エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidCharacter, '_');
            }

            return string.IsNullOrWhiteSpace(name) ? "AutoGameSettings" : name;
        }
    }
}
