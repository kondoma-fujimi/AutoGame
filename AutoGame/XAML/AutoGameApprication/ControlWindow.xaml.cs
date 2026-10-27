using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AutoGameApprication
{
    public partial class ControlWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private SwitchController? _switchController;
        private AutoGameWindow? _autoGameWindow;
        private const int ButtonCount = 5; // ボタンの分割数です。
        private const double StrokeThickness = 3; // 分割線と外周線の太さです。
        private const double WindowPadding = 16; // 外周からボタン領域までの幅です。
        private const double GapDegrees = 0; // 分割領域の角度方向の隙間です。

        private static readonly string[] ButtonLabels =
        {
            "接続\n終了", // 1番目のボタン名です。
            "撮影", // 2番目のボタン名です。
            "録画", // 3番目のボタン名です。
            "Manual", // 4番目のボタン名です。
            "Auto" // 5番目のボタン名です。
        };

        private static readonly string[] ButtonToolTips =
        {
            "キャプチャボード接続 / 終了", // 1番目のツールチップです。
            "スクリーンショット", // 2番目のツールチップです。
            "録画", // 3番目のツールチップです。
            "Manual操作", // 4番目のツールチップです。
            "Auto操作" // 5番目のツールチップです。
        };

        public ControlWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent(); // XAMLで定義したコントロールを初期化します。
            Loaded += (_, _) => UpdateLayoutButtons(); // 表示後にボタンを初期配置します。
            SizeChanged += (_, _) => UpdateLayoutButtons(); // サイズ変更時にボタンを再配置します。
            Closed += (_, _) => StopManualControl();
        }

        private void UpdateLayoutButtons()
        {
            ButtonCanvas.Children.Clear(); // 前回描画したボタンと線を削除します。

            double width = RootGrid.ActualWidth; // 描画領域の実際の幅を取得します。
            double height = RootGrid.ActualHeight; // 描画領域の実際の高さを取得します。
            if (width <= 0 || height <= 0)
            {
                return; // まだサイズが確定していない場合は処理を終了します。
            }

            double centerX = width / 2; // 円の中心X座標を計算します。
            double centerY = height / 2; // 円の中心Y座標を計算します。
            double outerRadius = Math.Min(width, height) / 2 - StrokeThickness / 2; // 外周円の半径を計算します。
            double buttonRadius = outerRadius - WindowPadding; // ボタン領域の半径を計算します。

            var outerBackground = new Ellipse
            {
                Width = outerRadius * 2, // 黒い背景円の幅を設定します。
                Height = outerRadius * 2, // 黒い背景円の高さを設定します。
                Fill = Brushes.Black, // ボタン外周を黒色で塗りつぶします。
                IsHitTestVisible = false // 背景円がクリックを奪わないようにします。
            };
            Canvas.SetLeft(outerBackground, centerX - outerRadius); // 背景円の左位置を設定します。
            Canvas.SetTop(outerBackground, centerY - outerRadius); // 背景円の上位置を設定します。
            ButtonCanvas.Children.Add(outerBackground); // 背景円をCanvasに追加します。

            double sliceSweep = 360.0 / ButtonCount; // 1つの扇形が占める角度を計算します。

            for (int index = 0; index < ButtonCount; index++) // 5つの扇形を順番に作成します。
            {
                double centerAngle = -90 + index * sliceSweep; // 上方向を基準に扇形の中心角を計算します。
                double startAngle = centerAngle + GapDegrees / 2; // 扇形の開始角度を計算します。
                double sweep = sliceSweep - GapDegrees; // 扇形の角度範囲を計算します。
                double endAngle = startAngle + sweep; // 扇形の終了角度を計算します。

                Point startPoint = PointOnCircle(centerX, centerY, buttonRadius, startAngle); // 外周上の開始点を計算します。
                Point endPoint = PointOnCircle(centerX, centerY, buttonRadius, endAngle); // 外周上の終了点を計算します。

                var segment = new StreamGeometry(); // 分割線用の図形を作成します。
                using (StreamGeometryContext context = segment.Open()) // 図形の描画コンテキストを開きます。
                {
                    context.BeginFigure(startPoint, false, false); // 扇形の外周線を開始します。
                    context.ArcTo( // 円弧を描画します。
                        endPoint,
                        new Size(buttonRadius, buttonRadius),
                        0,
                        sweep > 180,
                        SweepDirection.Clockwise,
                        true,
                        true);
                    context.LineTo(new Point(centerX, centerY), true, true); // 円弧の終了点から中心へ線を引きます。
                    context.LineTo(startPoint, true, true); // 中心から開始点へ線を引きます。
                }
                segment.Freeze(); // 作成した図形を読み取り専用にして描画性能を上げます。

                Rect buttonBounds = GetSectorBounds( // ボタンを配置する矩形範囲を計算します。
                    centerX,
                    centerY,
                    buttonRadius,
                    startAngle,
                    sweep);

                Geometry buttonClip = CreateSectorGeometry( // ボタンを扇形の形状に切り抜きます。
                    centerX - buttonBounds.Left,
                    centerY - buttonBounds.Top,
                    buttonRadius,
                    startAngle,
                    sweep);

                int buttonIndex = index; // クリック時にも正しいボタン番号を使えるようにコピーします。
                string buttonName = GetButtonLabel(buttonIndex); // 現在のボタン名を取得します。
                var button = new Button // 扇形ボタンを作成します。
                {
                    Width = buttonBounds.Width, // ボタンの幅を設定します。
                    Height = buttonBounds.Height, // ボタンの高さを設定します。
                    Content = buttonName, // ボタンに名前を表示します。
                    ToolTip = ButtonToolTips[buttonIndex], // マウスを重ねた時の説明を設定します。
                    FontWeight = FontWeights.Bold, // 文字を太字にします。
                    Background = Brushes.Black, // ボタンの背景を黒色にします。
                    Foreground = Brushes.White, // ボタンの文字を白色にします。
                    FontSize = 14, // ボタン文字の大きさを設定します。
                    Padding = new Thickness(2), // ボタン内側の余白を設定します。
                    Clip = buttonClip // ボタンを扇形に切り抜きます。
                };

                button.Click += (_, _) => ExecuteButton(buttonIndex); // クリック時に対応する操作を実行します。
                Canvas.SetLeft(button, buttonBounds.Left); // ボタンの左位置を設定します。
                Canvas.SetTop(button, buttonBounds.Top); // ボタンの上位置を設定します。
                ButtonCanvas.Children.Add(button); // ボタンをCanvasに追加します。

                ButtonCanvas.Children.Add(new Path // 扇形の境界線をCanvasに追加します。
                {
                    Data = segment, // 作成した分割線の図形を設定します。
                    Stroke = Brushes.LightSteelBlue, // 分割線の色を設定します。
                    StrokeThickness = StrokeThickness, // 分割線の太さを設定します。
                    Fill = Brushes.Transparent, // 図形内部は塗りつぶしません。
                    IsHitTestVisible = false, // 線がボタンのクリックを邪魔しないようにします。
                    StrokeStartLineCap = PenLineCap.Round, // 線の開始端を丸くします。
                    StrokeEndLineCap = PenLineCap.Round // 線の終了端を丸くします。
                });
            }

            var outerCircle = new Ellipse // 最外周の円を作成します。
            {
                Width = outerRadius * 2, // 外周円の幅を設定します。
                Height = outerRadius * 2, // 外周円の高さを設定します。
                Stroke = Brushes.LightSteelBlue, // 外周線の色を設定します。
                StrokeThickness = StrokeThickness, // 外周線の太さを設定します。
                Fill = Brushes.Transparent, // 外周円の内部は背景円を使用します。
                IsHitTestVisible = false // 外周円がドラッグ操作を邪魔しないようにします。
            };
            Canvas.SetLeft(outerCircle, centerX - outerRadius); // 外周円の左位置を設定します。
            Canvas.SetTop(outerCircle, centerY - outerRadius); // 外周円の上位置を設定します。
            ButtonCanvas.Children.Add(outerCircle); // 外周円をCanvasに追加します。
        }

        private string GetButtonLabel(int index)
        {
            return index switch
            {
                0 => _mainWindow.IsCapturing ? "接続終了" : "接続",
                2 => _mainWindow.IsRecording ? "録画停止" : ButtonLabels[index],
                3 => _switchController is null ? ButtonLabels[index] : "Manual停止",
                _ => ButtonLabels[index]
            };
        }

        private void ExecuteButton(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        _mainWindow.ToggleCapture();
                        break;
                    case 1:
                        _mainWindow.SaveScreenshot();
                        break;
                    case 2:
                        bool wasRecording = _mainWindow.IsRecording; // 停止操作前に録画中か確認します。
                        _mainWindow.ToggleRecording();
                        if (wasRecording && !_mainWindow.IsRecording) // 録画が停止した場合だけ通知します。
                        {
                            MessageBox.Show(this, "録画を停止しました。", "録画", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    case 3:
                        ToggleManualControl();
                        break;
                    case 4:
                        ShowAutoGameWindow();
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "操作エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateLayoutButtons();
        }

        private void ToggleManualControl()
        {
            if (_switchController is not null)
            {
                StopManualControl();
                return;
            }

            var controller = new SwitchController();
            controller.Disconnected += (_, _) =>
            {
                if (ReferenceEquals(_switchController, controller))
                {
                    _switchController = null;
                    UpdateLayoutButtons();
                }
            };
            controller.Start();
            _switchController = controller;
        }

        private void StopManualControl()
        {
            _switchController?.Dispose();
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

        private static Geometry CreateSectorGeometry( // ボタン用の扇形クリップを作成します。
            double centerX, // 扇形の中心X座標です。
            double centerY, // 扇形の中心Y座標です。
            double radius, // 扇形の半径です。
            double startAngle, // 扇形の開始角度です。
            double sweep) // 扇形の角度範囲です。
        {
            Point startPoint = PointOnCircle(centerX, centerY, radius, startAngle); // 扇形の開始点を計算します。
            Point endPoint = PointOnCircle(centerX, centerY, radius, startAngle + sweep); // 扇形の終了点を計算します。

            var geometry = new StreamGeometry(); // クリップ用の図形を作成します。
            using (StreamGeometryContext context = geometry.Open()) // 図形の描画コンテキストを開きます。
            {
                context.BeginFigure(startPoint, true, true); // 扇形の描画を開始します。
                context.ArcTo( // 外周の円弧を描画します。
                    endPoint,
                    new Size(radius, radius),
                    0,
                    sweep > 180,
                    SweepDirection.Clockwise,
                    true,
                    true);
                context.LineTo(new Point(centerX, centerY), true, true); // 円弧から中心へ戻します。
            }

            geometry.Freeze(); // クリップ図形を読み取り専用にします。
            return geometry; // 作成した扇形を返します。
        }

        private static Rect GetSectorBounds( // 扇形を囲む矩形範囲を計算します。
            double centerX, // 扇形の中心X座標です。
            double centerY, // 扇形の中心Y座標です。
            double radius, // 扇形の半径です。
            double startAngle, // 扇形の開始角度です。
            double sweep) // 扇形の角度範囲です。
        {
            double left = centerX; // 左端の初期値を設定します。
            double top = centerY; // 上端の初期値を設定します。
            double right = centerX; // 右端の初期値を設定します。
            double bottom = centerY; // 下端の初期値を設定します。

            for (int step = 0; step <= 72; step++) // 円弧上を72分割して範囲を調べます。
            {
                Point point = PointOnCircle( // 円弧上の現在位置を計算します。
                    centerX,
                    centerY,
                    radius,
                    startAngle + sweep * step / 72.0);

                left = Math.Min(left, point.X); // 左端を更新します。
                top = Math.Min(top, point.Y); // 上端を更新します。
                right = Math.Max(right, point.X); // 右端を更新します。
                bottom = Math.Max(bottom, point.Y); // 下端を更新します。
            }

            return new Rect(new Point(left, top), new Point(right, bottom)); // ボタン配置用の範囲を返します。
        }

        private static Point PointOnCircle(double centerX, double centerY, double radius, double angle) // 円周上の座標を計算します。
        {
            double radians = angle * Math.PI / 180; // 角度をラジアンへ変換します。
            return new Point( // 円周上の座標を返します。
                centerX + Math.Cos(radians) * radius,
                centerY + Math.Sin(radians) * radius);
        }
        // タイトルバーが無いのでドラッグで移動できるようにする
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsButtonSource(e.OriginalSource as DependencyObject)) // ボタン上のクリックか確認します。
            {
                return; // ボタン操作をウィンドウ移動として処理しません。
            }

            if (e.ButtonState == MouseButtonState.Pressed) // マウス左ボタンが押されたか確認します。
            {
                try { DragMove(); } catch { /* ウィンドウ移動できない場合は何もしません。 */ }
            }
        }

        private static bool IsButtonSource(DependencyObject? source) // クリック元がボタン内か確認します。
        {
            while (source is not null) // 親要素を順番に確認します。
            {
                if (source is Button) // ボタン要素に到達したか確認します。
                {
                    return true; // ボタン上のクリックであることを返します。
                }

                source = VisualTreeHelper.GetParent(source); // クリック元の親要素を取得します。
            }

            return false; // ボタン以外からのクリックであることを返します。
        }
    }
}