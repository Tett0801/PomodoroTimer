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
        private readonly Stopwatch stopwatch = new Stopwatch();
        private int countMax = 25 * 60;
        private string statement = "Ready";
        private bool isStarting = false;

        // 指定した感覚ごとに定期的に実行するタイマー
        public DispatcherTimer timer = new DispatcherTimer();
        

        public MainWindow()
        {
            InitializeComponent();
        }

        // 指定した間隔ごとに処理を実行
        private void Timer_Tick(object sender, EventArgs e)
        {
            var totalSeconds = ((int)this.stopwatch.Elapsed.TotalSeconds);
            
            RestSecond(totalSeconds, out int restSecond);

            // 周期を設定
            TimeSpan countTime = new TimeSpan(0, 0, (restSecond));

            // 残り時間を画面に表示
            if (restSecond >= 0)
            {
                timerTextBlock.Text = countTime.ToString(@"mm\:ss");
            }

            // タイマーが0秒になった場合
            if (restSecond == 0)
            {
                statement = "Finish";
                // ストップウォッチを停止
                stopwatch.Stop();
                stopwatch.Reset();
                // 状態ランプを変更
                ChangeStateLampToNext();
                TryGetCountMaxTime(out countMax);
                timer.Stop();
            }
        }

        // 計測時間の最大値を取得
        // Try get the max value to measure
        private void TryGetCountMaxTime(out int countMax)
        {
            // 状態ランプがオレンジ(休憩中)の場合、計測時間を5分とする
            if (statement == "ReadyRest")
            {
                // debugのため時間短縮
                countMax = 5 * 60;
                // countMax = 5;
            }
            else
            {
                // debugのため時間短縮
                countMax = 25 * 60;
                // countMax = 3;
            }
        }

        // 残り時間を取得
        private void RestSecond(int totalSeconds, out int restSecond)
        {
            restSecond = countMax - totalSeconds;
        }

        // 残り時間に応じて、状態ランプを変更
        private void ChangeStateLampToNext ()
        {
            if (statement == "Finish")
            {
                if (stateLamp.Fill == Brushes.Orange)
                {
                    statement = "Ready";
                    stateLamp.Fill = Brushes.LightGreen;
                }
                else if (stateLamp.Fill == Brushes.Red)
                {
                    statement = "ReadyRest";
                    stateLamp.Fill = Brushes.Orange;
                }

                buttonStartStop.Content = "Start";
                isStarting = false;
            }
        }

        // Start, Stopボタンクリックによる動作/テキスト変更
        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (isStarting)
            {
                stopwatch.Stop();
                ChangeTextToStart();
                // タイマーをストップ
                timer.Stop();
                isStarting = false;
            }

            else
            {
                if (statement == "Ready")
                {
                    statement = "Working";
                }
                else if (statement == "ReadyRest")
                {
                    statement = "Resting";
                }
                
                ChangeStateLampToRed(); 
                stopwatch.Start();
                ChangeTextToStop();

                // 間隔を1sごとに設定
                timer.Interval = new TimeSpan(0, 0, 1);
                // TryGetCountMaxTime(out countMax);
                timer.Tick += Timer_Tick;
                // タイマーをスタート
                timer.Start();
                isStarting = true;
            }
        }

        // ボタンテキストをStartに変更
        private void ChangeTextToStart()
        {
            buttonStartStop.Content = "Start";
        }

        // ボタンテキストをStopに変更
        private void ChangeTextToStop()
        {
            buttonStartStop.Content = "Stop";
        }

        // 状態ランプを緑から赤に変更
        private void ChangeStateLampToRed()
        {
            //if (stateLamp.Fill == Brushes.LightGreen)
            if (statement == "Working")
            {
                stateLamp.Fill = Brushes.Red;
            }
        }

        // Resetボタンをクリック
        // Timerを待機状態にリセットさせる
        // 緑の状態ランプを点灯させる
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            statement = "Ready";
            stopwatch.Reset();
            stateLamp.Fill = Brushes.LightGreen;
            buttonStartStop.Content = "Start";
            timer.Start();
            isStarting = false;
        }
    }
}
