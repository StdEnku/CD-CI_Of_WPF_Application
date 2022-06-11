using AnimatedContentControlLib.Core.Constants;
using AnimatedContentControlLib.Core.Messengers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;
using Constants = PrismBlankApp.Constants;

namespace PrismBlankApp.ViewModels
{
    /// <summary>
    /// MainWindow用のViewModel
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private IRegionManager _regionManager;

        #region RootRegionでの画面遷移用コマンド
        /// <summary>
        /// RootRegionでの画面遷移用コマンド
        /// </summary>
        public DelegateCommand<string> NavigateRootRegionCommand { get; private set; }

        private void navigateRootRegion(string nextViewName)
            => this._regionManager.RequestNavigate(Constants.RegionNames.RootRegion, nextViewName);
        #endregion

        #region RootRegionでのGoBackコマンド
        /// <summary>
        /// RootRegionでのGoBackコマンド
        /// </summary>
        public DelegateCommand GoBackRootRegionCommand { get; private set; }

        private void goBackRootRegion()
        {
            var navigationService = this._regionManager.Regions[Constants.RegionNames.RootRegion].NavigationService;
            var journal = navigationService.Journal;
            journal.GoBack();
        }

        private bool canGoBackRootRegion()
        {
            var isRootRegionExist = this._regionManager.Regions.ContainsRegionWithName(Constants.RegionNames.RootRegion);
            Debug.Assert(isRootRegionExist);

            var navigationService = this._regionManager.Regions[Constants.RegionNames.RootRegion].NavigationService;
            var journal = navigationService.Journal;
            return journal.CanGoBack;
        }
        #endregion

        #region RootRegionでのGoForwardコマンド
        /// <summary>
        /// RootRegionでのGoForwardコマンド
        /// </summary>
        public DelegateCommand GoForwardRootRegionCommand { get; private set; }

        private void goForwardRootRegion()
        {
            var navigationService = this._regionManager.Regions[Constants.RegionNames.RootRegion].NavigationService;
            var journal = navigationService.Journal;
            journal.GoForward();
        }

        private bool canGoForwardRootRegion()
        {
            var isRootRegionExist = this._regionManager.Regions.ContainsRegionWithName(Constants.RegionNames.RootRegion);
            Debug.Assert(isRootRegionExist);

            var navigationService = this._regionManager.Regions[Constants.RegionNames.RootRegion].NavigationService;
            var journal = navigationService.Journal;
            return journal.CanGoForward;
        }
        #endregion

        #region RootRegionでの画面遷移時実行されるコマンド
        public DelegateCommand ContnetChangedCommand { get; private set; }
        private void contentChanged()
        {
            this.GoBackRootRegionCommand.RaiseCanExecuteChanged();
            this.GoForwardRootRegionCommand.RaiseCanExecuteChanged();
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="regionManager">DIコンテナから注入されるRegionManager</param>
        public MainWindowViewModel(IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            this.NavigateRootRegionCommand = new DelegateCommand<string>(this.navigateRootRegion);
            this.GoBackRootRegionCommand = new DelegateCommand(this.goBackRootRegion, this.canGoBackRootRegion);
            this.GoForwardRootRegionCommand = new DelegateCommand(this.goForwardRootRegion, this.canGoForwardRootRegion);
            this.ContnetChangedCommand = new DelegateCommand(this.contentChanged);
        }
    }
}