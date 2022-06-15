using Settings = PrismBlankApp.Properties.Settings;

namespace PrismBlankApp.Services
{
    /// <summary>
    /// Settings.settingsの内容をViewModelで読み書きするためのサービス
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// MainWindowの高さを保存するためのプロパティ
        /// </summary>
        public double MainWindowHeight
        {
            get => Settings.Default.MainWindowHeight;
            set => Settings.Default.MainWindowHeight = value;
        }

        /// <summary>
        /// MainWindowの幅を保存するためのプロパティ
        /// </summary>
        public double MainWindowWidth
        {
            get => Settings.Default.MainWindowWidth;
            set => Settings.Default.MainWindowWidth = value;
        }

        /// <summary>
        /// MainWindowの横方向の位置を保存するためのプロパティ
        /// </summary>
        public double MainWindowLeft
        {
            get => Settings.Default.MainWindowLeft;
            set => Settings.Default.MainWindowLeft = value;
        }

        /// <summary>
        /// MainWindowの縦方向の位置を保存するためのプロパティ
        /// </summary>
        public double MainWindowTop
        {
            get => Settings.Default.MainWindowTop;
            set => Settings.Default.MainWindowTop = value;
        }

        /// <summary>
        /// MainWindowのWindowStateがNormalか否かを保存するためのプロパティ
        /// </summary>
        public bool MainWindowIsNormalState
        {
            get => Settings.Default.MainWindowIsNormalState;
            set => Settings.Default.MainWindowIsNormalState = value;
        }

        /// <summary>
        /// プロパティを保存するためのメソッド
        /// </summary>
        public void Save()
        {
            Settings.Default.Save();
        }
    }
}