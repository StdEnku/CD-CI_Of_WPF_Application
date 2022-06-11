using Prism.Ioc;
using System.Windows;
using Constants = PrismBlankApp.Constants;
using Views = PrismBlankApp.Views;

namespace PrismBlankApp
{
    /// <summary>
    /// エントリーポイント
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// 起動時表示するWindowを指定するメソッド
        /// </summary>
        /// <returns></returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();
        }

        /// <summary>
        /// DIコンテナへの登録用メソッド
        /// </summary>
        /// <param name="containerRegistry">抽象化したDIコンテナオブジェクト</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.InitView>(Constants.ViewNames.ItitView);
        }
    }
}