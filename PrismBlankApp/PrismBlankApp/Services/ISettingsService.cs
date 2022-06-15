namespace PrismBlankApp.Services
{
    /// <summary>
    /// Settings.settingsの内容をViewModelで読み書きするためのサービス用インターフェース
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// MainWindowの高さを保存するためのプロパティ
        /// </summary>
        double MainWindowHeight { get; set; }

        /// <summary>
        /// MainWindowの幅を保存するためのプロパティ
        /// </summary>
        double MainWindowWidth { get; set; }

        /// <summary>
        /// MainWindowの横方向の位置を保存するためのプロパティ
        /// </summary>
        double MainWindowLeft { get; set; }

        /// <summary>
        /// MainWindowの縦方向の位置を保存するためのプロパティ
        /// </summary>
        double MainWindowTop { get; set; }

        /// <summary>
        /// MainWindowのWindowStateがNormalか否かを保存するためのプロパティ
        /// </summary>
        bool MainWindowIsNormalState { get; set; }

        /// <summary>
        /// プロパティを保存するためのメソッド
        /// </summary>
        void Save();
    }
}