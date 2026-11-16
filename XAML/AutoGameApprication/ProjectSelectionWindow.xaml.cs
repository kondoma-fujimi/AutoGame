using System.Windows;

namespace AutoGameApprication
{
    public partial class ProjectSelectionWindow : Window
    {
        public ProjectSelectionWindow()
        {
            InitializeComponent();
        }

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new GameSettings();
            var comPortWindow = new ComPortSelectionWindow(settings.ComPortName)
            {
                Owner = this
            };

            if (comPortWindow.ShowDialog() != true ||
                string.IsNullOrWhiteSpace(comPortWindow.SelectedPort))
            {
                return;
            }

            settings.ComPortName = comPortWindow.SelectedPort;
            var gameWindow = new MainWindow(settings);
            Application.Current.MainWindow = gameWindow;
            gameWindow.Show();
            Close();
        }
    }
}
