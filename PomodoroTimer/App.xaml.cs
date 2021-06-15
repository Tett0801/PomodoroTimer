using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PomodoroTimer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // MainWindowのインスタンス化
            var mainWindow = new MainWindow();
            // ディスプレイの高さを取得
            var dispHeight = SystemParameters.PrimaryScreenHeight;
            // ディスプレイの幅を取得
            var dispWidth = SystemParameters.PrimaryScreenWidth;
            // MainWindowの高さを取得
            var appHeight = mainWindow.Height;
            // MainWindowの幅を取得
            var appWidth = mainWindow.Width;
            // MainWindowを表示する縦位置を指定
            mainWindow.Top = dispHeight - appHeight - 35;
            // MainWindowを表示する横位置を指定
            mainWindow.Left = dispWidth - appWidth;
            // MainWindowを表示
            mainWindow.ShowDialog();
        }
    }
}
