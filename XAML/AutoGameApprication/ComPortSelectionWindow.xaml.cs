using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace AutoGameApprication
{
    public partial class ComPortSelectionWindow : Window
    {
        private readonly string _configuredPort;

        public string? SelectedPort { get; private set; }

        public ComPortSelectionWindow(string configuredPort)
        {
            _configuredPort = configuredPort;
            InitializeComponent();
            RefreshPorts();
        }

        private void RefreshPorts()
        {
            string? selectedPort = ComPortComboBox.SelectedItem as string ?? _configuredPort;
            string[] detectedPorts = SerialPort.GetPortNames()
                .OrderBy(portName => portName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var ports = detectedPorts.ToList();
            if (!ports.Contains(_configuredPort, StringComparer.OrdinalIgnoreCase))
            {
                ports.Add(_configuredPort);
            }

            ComPortComboBox.ItemsSource = ports;
            ComPortComboBox.SelectedItem = ports.FirstOrDefault(
                portName => string.Equals(portName, selectedPort, StringComparison.OrdinalIgnoreCase));

            bool configuredPortDetected = detectedPorts.Contains(
                _configuredPort,
                StringComparer.OrdinalIgnoreCase);
            PortStatusText.Text = configuredPortDetected
                ? "使用するCOMポートを選択してください。"
                : $"{_configuredPort}は設定済みポートとして候補に表示していますが、Windowsでは未検出です。接続を確認して「再取得」を押してください。";
        }

        private void ComPortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateButton.IsEnabled = ComPortComboBox.SelectedItem is string;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPorts();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComPortComboBox.SelectedItem is not string portName)
            {
                MessageBox.Show(
                    this,
                    "COMポートを選択してください。",
                    "COMポート選択",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            SelectedPort = portName;
            DialogResult = true;
        }
    }
}
