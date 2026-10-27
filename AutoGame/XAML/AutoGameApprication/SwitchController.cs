using System.Buffers.Binary;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace AutoGameApprication
{
    internal sealed class SwitchController : IDisposable
    {
        private const int HatNone = 0x08;
        private readonly SerialPort _serialPort = new("COM3", 115200)
        {
            WriteTimeout = 500
        };
        private readonly DispatcherTimer _timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(20)
        };

        public SwitchController()
        {
            _timer.Tick += SendReport;
        }

        public event EventHandler? Disconnected;

        public void Start()
        {
            try
            {
                if (!SerialPort.GetPortNames().Contains("COM3", StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("COM3ポートが見つかりませんでした。");
                }

                _serialPort.Open();
                _timer.Start();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private void SendReport(object? sender, EventArgs e)
        {
            try
            {
                byte[] report = BuildReport();
                _serialPort.Write(report, 0, report.Length);
            }
            catch (Exception exception)
            {
                Dispose();
                Disconnected?.Invoke(this, EventArgs.Empty);
                MessageBox.Show($"コントローラー通信を終了しました。\n{exception.Message}", "通信エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static byte[] BuildReport()
        {
            ushort buttons = 0;
            AddButton(ref buttons, "H", 1 << 2);
            AddButton(ref buttons, "J", 1 << 1);
            AddButton(ref buttons, "K", 1 << 3);
            AddButton(ref buttons, "L", 1 << 0);
            AddButton(ref buttons, "Y", 1 << 4);
            AddButton(ref buttons, "U", 1 << 5);
            AddButton(ref buttons, "I", 1 << 6);
            AddButton(ref buttons, "O", 1 << 7);
            AddButton(ref buttons, "T", 1 << 9);
            AddButton(ref buttons, "R", 1 << 8);
            AddButton(ref buttons, "F", 1 << 12);
            AddButton(ref buttons, "G", 1 << 13);

            byte hat = IsKeyPressed(0x38) ? (byte)0x00
                : IsKeyPressed(0x36) ? (byte)0x02
                : IsKeyPressed(0x32) ? (byte)0x04
                : IsKeyPressed(0x34) ? (byte)0x06
                : (byte)HatNone;

            byte leftX = IsKeyPressed(0x41) ? (byte)0x00 : IsKeyPressed(0x44) ? (byte)0xFF : (byte)0x80;
            byte leftY = IsKeyPressed(0x57) ? (byte)0x00 : IsKeyPressed(0x53) ? (byte)0xFF : (byte)0x80;
            byte rightX = IsKeyPressed(0xBA) ? (byte)0x00 : IsKeyPressed(0xDD) ? (byte)0xFF : (byte)0x80;
            byte rightY = IsKeyPressed(0x32) && IsKeyPressed(0x10) ? (byte)0x00
                : IsKeyPressed(0xBA) && IsKeyPressed(0x10) ? (byte)0xFF
                : (byte)0x80;

            byte[] report = new byte[8];
            BinaryPrimitives.WriteUInt16LittleEndian(report, buttons);
            report[2] = hat;
            report[3] = leftX;
            report[4] = leftY;
            report[5] = rightX;
            report[6] = rightY;
            report[7] = 0;
            return report;
        }

        private static void AddButton(ref ushort buttons, string key, int mask)
        {
            if (IsKeyPressed((int)key[0]))
            {
                buttons |= (ushort)mask;
            }
        }

        private static bool IsKeyPressed(int virtualKey)
        {
            return (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
        }

        public void Dispose()
        {
            _timer.Stop();
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            _serialPort.Dispose();
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int virtualKey);
    }
}