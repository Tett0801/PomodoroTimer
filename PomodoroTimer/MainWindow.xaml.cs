using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PomodoroTimer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // フィールド

        // 状態変数を定義
        private Statement _statement;
        // 状態ランプカラーを定義
        private StatementLampColor _statementLampColor;
        // タイマー動作中か否かの判定フラッグ
        private bool isStarting = false;
        // タイマーの最大時間
        private int _maxTime = 25 * 60;
        // 休憩時間
        private readonly int _restTime = 5;
        // private readonly int restTime = 5 * 60;
        // 作業時間
        private readonly int _workTime = 10;
        // private readonly int workTime = 25 * 60;
        // ストップウォッチインスタンスを生成
        private readonly Stopwatch stopwatch = new Stopwatch();
        // 指定した感覚ごとに定期的に実行するタイマー
        public DispatcherTimer timer = new DispatcherTimer();

        // コンストラクタ―
        public MainWindow()
        {
            InitializeComponent();
        }

        // メソッド

        // 指定した間隔ごとに処理を実行
        private void DoPerCycleTime(object sender, EventArgs e)
        {
            // ストップウォッチがスタートしてからの経過時間を取得
            var elapsedTime = ((int)this.stopwatch.Elapsed.TotalSeconds);
            
            // タイマーの残り時間を取得
            GetRemainTime(_maxTime, elapsedTime, out int remainTime);

            // 残り時間を画面に表示するために
            // タイマーの残り時間からTimeSpanオブジェクトを生成
            TimeSpan remainTimeObject = new TimeSpan(0, 0, (remainTime));

            // 残り時間を画面に表示
            if (remainTime >= 0)
            {
                timerTextBlock.Text = remainTimeObject.ToString(@"mm\:ss");
            }

            // タイマーが0秒になった場合
            if (remainTime < 0)
            {
                // 状態を"Finish"にする
                _statement = Statement.Finish;
                // ストップウォッチを停止する
                stopwatch.Stop();
                // ストップウォッチをリセットする
                stopwatch.Reset();
                // 状態表示ランプを変更する
                ChangeStateLampToNext();
                // 状態に応じた最大時間を取得する
                TryGetMaxTime(_statement, out _maxTime);
            }
        }

        // 計測時間の最大値を取得
        private void TryGetMaxTime(Statement statement, out int maxTime)
        {
            // 状態ランプがオレンジ(休憩中)の場合、計測時間を5分とする
            switch(statement)
            {
                case Statement.ReadyRest:
                    maxTime = _restTime;
                    break;
                default:
                    maxTime = _workTime;
                    break;
            }
        }

        // タイマーの残り時間を取得する
        private void GetRemainTime(int maxTime, int elapsedTime, out int remainTime)
        {
            remainTime = maxTime - elapsedTime;
        }

        // 残り時間に応じて状態ランプを変更する
        private void ChangeStateLampToNext()
        {
            // 状態が"Finish"の場合
            if (_statement == Statement.Finish)
            {
                // フラッグを停止中に変更する
                isStarting = false;
                // ボタンの表示テキストを"Start"に変更する
                buttonStartStop.Content = StartStopButtonText.Start;

                // 状態ランプの色に応じて
                // 状態と状態ランプの色を変更する
                switch(_statementLampColor)
                {
                    // 状態ランプの色がオレンジの場合
                    case StatementLampColor.Orange:
                        _statement = Statement.Ready;
                        _statementLampColor = StatementLampColor.LightGreen;
                        statementLamp.Fill = Brushes.LightGreen;
                        break;
                    // 状態ランプの色が赤色の場合
                    case StatementLampColor.Red:
                        _statement = Statement.ReadyRest;
                        _statementLampColor = StatementLampColor.Orange;
                        statementLamp.Fill = Brushes.Orange;
                        break;
                }
            }
        }

        // ボタンの表示テキストを"Start"に変更する
        private void ChangeTextToStart()
        {
            buttonStartStop.Content = StartStopButtonText.Start;
        }

        // ボタンの表示テキストをStopに変更する
        private void ChangeTextToStop()
        {
            buttonStartStop.Content = StartStopButtonText.Stop;
        }

        // 状態ランプの色を緑から赤に変更する
        private void ChangeStateLampToRed()
        {
            if (_statement == Statement.Working)
            {
                _statementLampColor = StatementLampColor.Red;
                statementLamp.Fill = Brushes.Red;
            }
        }

        // Start, Stopボタンクリックによる動作/テキスト変更
        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            // タイマーが動作中の場合
            if (isStarting)
            {
                // ストップウォッチを停止する
                stopwatch.Stop();
                // ボタンのテキストを"Start"に変更する
                ChangeTextToStart();
                // フラッグを停止中にする
                isStarting = false;
            }

            // タイマーが停止中の場合
            else
            {
                // 最大時間を取得する
                TryGetMaxTime(_statement, out _maxTime);

                // 現在の状態に応じて、状態を変更する
                switch (_statement)
                {
                    case Statement.Ready:
                        _statement = Statement.Working;
                        break;
                    case Statement.ReadyRest:
                        _statement = Statement.Resting;
                        break;
                }

                // 状態ランプの色を赤色に変更する
                ChangeStateLampToRed();
                // ストップウォッチをスタートする
                stopwatch.Start();
                // ボタンの表示を"Stop"に変更する
                ChangeTextToStop();
                // タイマーの実行周期を1秒とする
                timer.Interval = new TimeSpan(0, 0, 1);
                // タイマーの実行周期が経過した際に実行する動作を設定する
                timer.Tick += DoPerCycleTime;
                // タイマーをスタートする
                timer.Start();
                // フラッグを開始中にする
                isStarting = true;
            }
        }

        // Resetボタンをクリック
        // Timerを待機状態にリセットさせる
        // 緑の状態ランプを点灯させる
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            // 状態を"Ready"に変更する
            _statement = Statement.Ready;
            // ストップウォッチをリセットする
            stopwatch.Reset();
            // 状態ランプの色を緑色に変更する
            _statementLampColor = StatementLampColor.LightGreen;
            statementLamp.Fill = Brushes.LightGreen;
            // ボタンの表示テキストを"Start"に変更する
            buttonStartStop.Content = StartStopButtonText.Start;
            // タイマーの最大時間を取得する
            TryGetMaxTime(_statement, out _maxTime);
            // フラッグを停止中に変更する
            isStarting = false;
        }

        // 状態の列挙
        private enum Statement
        {
            Ready,
            ReadyRest,
            Resting,
            Working,
            Finish,
        }

        // 状態ランプの色を列挙
        private enum StatementLampColor
        {
            LightGreen,
            Red,
            Orange,
        }

        // buttonStartStopの表示テキストを列挙
        private enum StartStopButtonText
        {
            Start,
            Stop,
        }
    }
}
