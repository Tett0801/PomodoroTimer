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
        // メンバー

        // 状態変数を定義
        private Statement _statement;
        // 状態ランプカラーを定義
        private StatementLampColor _statementLampColor;
        // タイマー動作中か否かの判定フラッグ
        private bool _isStarting = false;
        // タイマーの最大時間
        private int _maxTime = 25 * 60;
        // 休憩時間
        private readonly int _restTime = 5 * 60;
        // 作業時間
        private readonly int _workTime = 25 * 60;
        // ストップウォッチインスタンスを生成
        private readonly Stopwatch _stopwatch = new Stopwatch();
        // 指定した感覚ごとに定期的に実行するタイマー
        public DispatcherTimer _timer = new DispatcherTimer();

        // コンストラクタ―
        /// <summary>
        /// 画面を表示
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        // メソッド

        // 指定した間隔ごとに処理を実行
        /// <summary>
        /// 指定した周期ごとにタイマーの残り時間を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoPerCycleTime(object sender, EventArgs e)
        {
            // ストップウォッチがスタートしてからの経過時間を取得
            var elapsedTime = ((int)this._stopwatch.Elapsed.TotalSeconds);
            
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
                _stopwatch.Stop();
                // ストップウォッチをリセットする
                _stopwatch.Reset();
                // 状態表示ランプを変更する
                ChangeStateLampToNext();
                // 状態に応じた最大時間を取得する
                TryGetMaxTime(_statement, out _maxTime);
            }
        }

        /// <summary>
        /// 状態に応じたタイマーの最大時間を取得する
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="maxTime"></param>
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

        /// <summary>
        /// タイマーの残り時間を取得する
        /// </summary>
        /// <param name="maxTime"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="remainTime"></param>
        private void GetRemainTime(int maxTime, int elapsedTime, out int remainTime)
        {
            remainTime = maxTime - elapsedTime;
        }

        /// <summary>
        /// 残り時間に応じて状態ランプを変更する
        /// </summary>
        private void ChangeStateLampToNext()
        {
            // 状態が"Finish"の場合
            if (_statement == Statement.Finish)
            {
                // フラッグを停止中に変更する
                _isStarting = false;
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

        /// <summary>
        /// ボタンの表示テキストを"Start"に変更する
        /// </summary>
        private void ChangeTextToStart()
        {
            buttonStartStop.Content = StartStopButtonText.Start;
        }

        /// <summary>
        /// ボタンの表示テキストをStopに変更する
        /// </summary>
        private void ChangeTextToStop()
        {
            buttonStartStop.Content = StartStopButtonText.Stop;
        }

        /// <summary>
        /// 状態ランプの色を緑から赤に変更する
        /// </summary>
        private void ChangeStateLampToRed()
        {
            if (_statement == Statement.Working)
            {
                _statementLampColor = StatementLampColor.Red;
                statementLamp.Fill = Brushes.Red;
            }
        }

        /// <summary>
        /// Start, Stopボタンクリックによる動作/テキスト変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            // タイマーが動作中の場合
            if (_isStarting)
            {
                // ストップウォッチを停止する
                _stopwatch.Stop();
                // ボタンのテキストを"Start"に変更する
                ChangeTextToStart();
                // フラッグを停止中にする
                _isStarting = false;
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
                _stopwatch.Start();
                // ボタンの表示を"Stop"に変更する
                ChangeTextToStop();
                // タイマーの実行周期を1秒とする
                _timer.Interval = new TimeSpan(0, 0, 1);
                // タイマーの実行周期が経過した際に実行する動作を設定する
                _timer.Tick += DoPerCycleTime;
                // タイマーをスタートする
                _timer.Start();
                // フラッグを開始中にする
                _isStarting = true;
            }
        }

        /// <summary>
        /// リセットボタンのクリックに応じて
        /// タイマーを待機状態にリセットする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            // 状態を"Ready"に変更する
            _statement = Statement.Ready;
            // ストップウォッチをリセットする
            _stopwatch.Reset();
            // 状態ランプの色を緑色に変更する
            _statementLampColor = StatementLampColor.LightGreen;
            statementLamp.Fill = Brushes.LightGreen;
            // ボタンの表示テキストを"Start"に変更する
            buttonStartStop.Content = StartStopButtonText.Start;
            // タイマーの最大時間を取得する
            TryGetMaxTime(_statement, out _maxTime);
            // フラッグを停止中に変更する
            _isStarting = false;
        }

        /// <summary>
        /// 状態の列挙
        /// </summary>
        private enum Statement
        {
            Ready,
            ReadyRest,
            Resting,
            Working,
            Finish,
        }

        /// <summary>
        /// 状態ランプの色を列挙
        /// </summary>
        private enum StatementLampColor
        {
            LightGreen,
            Red,
            Orange,
        }

        /// <summary>
        /// buttonStartStopの表示テキストを列挙
        /// </summary>
        private enum StartStopButtonText
        {
            Start,
            Stop,
        }
    }
}
