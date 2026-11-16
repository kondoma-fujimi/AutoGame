using System.Windows;

namespace AutoGameApprication
{
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
            foreach (string entry in AppLog.GetEntries())
            {
                LogListBox.Items.Add(entry);
            }

            AppLog.EntryAdded += AppLog_EntryAdded;
            Closed += (_, _) => AppLog.EntryAdded -= AppLog_EntryAdded;
        }

        private void AppLog_EntryAdded(object? sender, string entry)
        {
            Dispatcher.Invoke(() =>
            {
                LogListBox.Items.Add(entry);
                LogListBox.ScrollIntoView(LogListBox.Items[^1]);
            });
        }
    }
}
