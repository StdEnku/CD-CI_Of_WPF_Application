using System.Windows;

namespace PrismBlankApp.Services
{
    /// <summary>
    /// Windowの操作をするためのサービス
    /// </summary>
    public class TitlebarOperationService : ITitlebarOperationService
    {
        /// <summary>
        /// アプリケーションを終了するメソッド
        /// </summary>
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// WindowStateをMinimizeにするためのメソッド
        /// </summary>
        public void Minimize()
        {
            var window = Application.Current.MainWindow;
            window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// WindowStateをMaximizeにするためのメソッド
        /// </summary>
        public void Maximize()
        {
            var window = Application.Current.MainWindow;
            window.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// WindowStateをNormalにするためのメソッド
        /// </summary>
        public void Normalize()
        {
            var window = Application.Current.MainWindow;
            window.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// 現在のWindowStateがNormalかどうかを確かめるためのメソッド
        /// </summary>
        /// <returns>NormalならTrue</returns>
        public bool IsNormalState()
        {
            var window = Application.Current.MainWindow;
            var result = window.WindowState == WindowState.Normal;
            return result;
        }
    }
}