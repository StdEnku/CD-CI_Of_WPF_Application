using System.Windows;

namespace PrismBlankApp.Services
{
    /// <summary>
    /// Windowの操作をするためのサービス用インターフェース
    /// </summary>
    public interface ITitlebarOperationService
    {
        /// <summary>
        /// アプリケーションを終了するメソッド
        /// </summary>
        void Shutdown();

        /// <summary>
        /// WindowStateをMinimizeにするためのメソッド
        /// </summary>
        void Minimize();

        /// <summary>
        /// WindowStateをMaximizeにするためのメソッド
        /// </summary>
        void Maximize();

        /// <summary>
        /// WindowStateをNormalにするためのメソッド
        /// </summary>
        void Normalize();

        /// <summary>
        /// 現在のWindowStateがNormalかどうかを確かめるためのメソッド
        /// </summary>
        /// <returns>NormalならTrue</returns>
        bool IsNormalState();
    }
}