using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoGameApprication
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            Loaded += MenuWindow_Loaded;
        }

        private void MenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var duration = new Duration(TimeSpan.FromMilliseconds(650));
            var easing = new BackEase
            {
                Amplitude = 0.2,
                EasingMode = EasingMode.EaseOut
            };

            var scaleAnimation = new DoubleAnimation
            {
                From = 0.18,
                To = 1.0,
                Duration = duration,
                EasingFunction = easing
            };

            var opacityAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                BeginTime = TimeSpan.FromMilliseconds(350)
            };

            OpenScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            OpenScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
            DeviceShell.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            var screenAnimation = new DoubleAnimation
            {
                From = 0.08,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                BeginTime = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var menuAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(350)),
                BeginTime = TimeSpan.FromMilliseconds(700),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            ScreenScale.BeginAnimation(ScaleTransform.ScaleYProperty, screenAnimation);
            MenuSurface.BeginAnimation(UIElement.OpacityProperty, menuAnimation);
            Loaded -= MenuWindow_Loaded;
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new MainWindow();
            Application.Current.MainWindow = gameWindow;
            gameWindow.Show();
            Close();
        }

        private void NotImplementedButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string menuName)
            {
                MessageBox.Show(
                    this,
                    $"{menuName}の画面はまだ実装されていません。",
                    menuName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsButtonSource(e.OriginalSource as DependencyObject))
            {
                return;
            }

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private static bool IsButtonSource(DependencyObject? source)
        {
            while (source is not null)
            {
                if (source is Button)
                {
                    return true;
                }

                source = VisualTreeHelper.GetParent(source);
            }

            return false;
        }
    }
}
