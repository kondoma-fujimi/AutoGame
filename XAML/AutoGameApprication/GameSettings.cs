namespace AutoGameApprication
{
    public sealed class GameSettings
    {
        public string GameTitle { get; set; } = "タイトル未設定";
        public string OutputDirectory { get; set; } = Environment.CurrentDirectory;
        public string ComPortName { get; set; } = "COM3";
        public int CaptureDeviceIndex { get; set; } = 0;
    }
}
