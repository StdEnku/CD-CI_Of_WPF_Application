using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;
using Constants = PrismBlankApp.Constants;

namespace PrismBlankApp.ViewModels
{
    /// <summary>
    /// ViewModel用基底クラス
    /// </summary>
    public class ViewModelBase : BindableBase, INavigationAware, IRegionMemberLifetime, IJournalAware
    {
        private IRegionNavigationService? _regionNavigationService;

        #region IRegionMemberLifetimeの実装
        /// <summary>
        /// 遷移後も本ViewModelに対応するViewのインスタンスを破棄しないか
        /// </summary>
        /// <remarks>
        /// trueなら遷移後本ViewModelに対応するViewのインスタンスは破棄されない
        /// falseなら遷移後本ViewModelに対応するViewのインスタンスは破棄される
        /// </remarks>
        public virtual bool KeepAlive => false;
        #endregion

        #region InavigationAwareの実装
        /// <summary>
        /// 再遷移後本ViewModelに対応するViewのインスタンスを再利用するか
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns>
        /// trueなら再遷移後本ViewModelに対応するViewの旧インスタンスは再利用される
        /// falseなら再選以後本ViewModelに対応するViewの新たなインスタンスが作成される
        /// </returns>
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
            => false;

        /// <summary>
        /// 本ViewModelに対応するViewから他の画面に遷移する直前に実行される処理
        /// </summary>
        /// <param name="navigationContext">画面遷移の情報を保持するオブジェクト</param>
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (this._regionNavigationService is not null)
            {
                this._regionNavigationService.Navigated -= this.onNavigated;
            }
        }

        /// <summary>
        /// 他の画面から本ViewModelに対応するViewへ遷移した直後実行される処理
        /// </summary>
        /// <param name="navigationContext">画面遷移の情報を保持するオブジェクト</param>
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._regionNavigationService = navigationContext.NavigationService;
            this._regionNavigationService.Navigated += this.onNavigated;
        }
        #endregion

        #region IJounalAwareの実装
        /// <summary>
        /// NavigationJournalに対応するViewを記録するかどうか
        /// </summary>
        /// <returns>trueなら記録する</returns>
        public virtual bool PersistInHistory() => true;
        #endregion

        #region 本ViewModelに対応するViewを表示しているRegionでの画面遷移用コマンド
        /// <summary>
        /// 本ViewModelに対応するViewを表示しているRegionでの画面遷移用コマンド
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; private set; }

        private void navigate(string nextViewName)
        {
            Debug.Assert(this._regionNavigationService is not null);
            this.onBeforeNavigate(nextViewName);
            this._regionNavigationService.RequestNavigate(nextViewName);
        }

        private bool canNavigate(string nextViewName)
            => this._regionNavigationService is not null;
        #endregion

        #region 本ViewModelに対応するViewを表示しているRegionでのGoBack用コマンド
        /// <summary>
        /// 本ViewModelに対応するViewを表示しているRegionでのGoBack用コマンド
        /// </summary>
        public DelegateCommand GoBackCommand { get; private set; }

        private void goBack()
        {
            Debug.Assert(this._regionNavigationService is not null);
            Debug.Assert(this._regionNavigationService.Journal is not null);
            this.onBeforeGoBack();
            this._regionNavigationService.Journal.GoBack();
        }

        private bool canGoBack()
        {
            var regionNavigationService = this._regionNavigationService;
            if (regionNavigationService is null)
            {
                return false;
            }

            return regionNavigationService.Journal.CanGoBack;
        }
        #endregion

        #region 本ViewModelに対応するViewを表示しているRegionでのGoForward用コマンド
        /// <summary>
        /// 本ViewModelに対応するViewを表示しているRegionでのGoForward用コマンド
        /// </summary>
        public DelegateCommand GoForwardCommand { get; private set; }

        private void goForward()
        {
            Debug.Assert(this._regionNavigationService is not null);
            Debug.Assert(this._regionNavigationService.Journal is not null);
            this.onBeforeGoForward();
            this._regionNavigationService.Journal.GoForward();
        }

        private bool canGoForward()
        {
            var regionNavigationService = this._regionNavigationService;
            if (regionNavigationService is null)
            {
                return false;
            }

            return regionNavigationService.Journal.CanGoForward;
        }
        #endregion

        /// <summary>
        /// NavigateCommand実行直前に実行されるメソッド
        /// 派生先で処理を書くことを想定している
        /// </summary>
        protected virtual void onBeforeNavigate(string nextViewName)
        {

        }

        /// <summary>
        /// GoBackCommand実行直前に実行されるメソッド
        /// 派生先で処理を書くことを想定している
        /// </summary>
        protected virtual void onBeforeGoBack()
        {

        }

        /// <summary>
        /// GoForwardCommand実行直前に実行されるメソッド
        /// 派生先で処理を書くことを想定している
        /// </summary>
        protected virtual void onBeforeGoForward()
        {

        }

        private void onNavigated(object? sender, EventArgs e)
        {
            this.NavigateCommand.RaiseCanExecuteChanged();
            this.GoBackCommand.RaiseCanExecuteChanged();
            this.GoForwardCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="regionManager">DIコンテナから注入されるRegionManager</param>
        public ViewModelBase()
        {
            this.NavigateCommand = new DelegateCommand<string>(this.navigate, this.canNavigate);
            this.GoBackCommand = new DelegateCommand(this.goBack, this.canGoBack);
            this.GoForwardCommand = new DelegateCommand(this.goForward, this.canGoForward);
        }
    }
}