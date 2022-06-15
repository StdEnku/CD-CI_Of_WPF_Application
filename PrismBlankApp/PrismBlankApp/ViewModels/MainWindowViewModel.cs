using AnimatedContentControlLib.Core.Constants;
using AnimatedContentControlLib.Core.Messengers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;
using Services = PrismBlankApp.Services;

namespace PrismBlankApp.ViewModels
{
    /// <summary>
    /// MainWindow用のViewModel
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private IRegionManager _regionManager;
        private Services.ISettingsService _settingsService;
        private Services.ITitlebarOperationService _titlebarOperationService;

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

        #region タイトルバーの最小化ボタンが押された際実行されるコマンド
        /// <summary>
        /// タイトルバーの最小化ボタンが押された際実行されるコマンド
        /// </summary>
        public DelegateCommand MinimizeButtonClickedCommand { get; private set; }
        private void minimizeButtonClicked()
        {
            this._titlebarOperationService.Minimize();
        }
        #endregion

        #region タイトルバーの最大化ボタンまたは普通サイズ化ボタンが押された際に実行されるコマンド
        /// <summary>
        /// タイトルバーの最大化ボタンまたは普通サイズ化ボタンが押された際に実行されるコマンド
        /// </summary>
        public DelegateCommand NormalizeOrMaximizeButtonClickedCommand { get; private set; }
        private void normalizeOrMaximizeButtonClicked()
        {
            var isNormalState = this._titlebarOperationService.IsNormalState();

            if (isNormalState)
            {
                this._titlebarOperationService.Maximize();
            }
            else
            {
                this._titlebarOperationService.Normalize();
            }
        }
        #endregion

        #region 終了ボタンが押された際に実行されるコマンド
        /// <summary>
        /// 終了ボタンが押された際に実行されるコマンド
        /// </summary>
        public DelegateCommand ShutdownButtonClickedCommand { get; private set; }
        private void shutdownButtonClickedCommand()
        {
            var isNormalState = this._titlebarOperationService.IsNormalState();
            if (isNormalState)
            {
                this._settingsService.MainWindowHeight = this.WindowHeight;
                this._settingsService.MainWindowWidth = this.WindowWidth;
                this._settingsService.MainWindowLeft = this.WindowLeft;
                this._settingsService.MainWindowTop = this.WindowTop;
                this._settingsService.MainWindowIsNormalState = true;
                this._settingsService.Save();
            }
            else
            {
                this._settingsService.MainWindowIsNormalState = false;
                this._settingsService.Save();
            }

            this._titlebarOperationService.Shutdown();
        }
        #endregion

        #region Windowの高さプロパティ
        private double _windowHeight;

        /// <summary>
        /// Windowの高さプロパティ
        /// </summary>
        public double WindowHeight
        {
            get => this._windowHeight;
            set => this.SetProperty(ref this._windowHeight, value);
        }
        #endregion

        #region Windowの幅プロパティ
        private double _windowWidth;

        /// <summary>
        /// Windowの幅プロパティ
        /// </summary>
        public double WindowWidth
        {
            get => this._windowWidth;
            set => this.SetProperty(ref this._windowWidth, value);
        }
        #endregion

        #region Windowの横軸の位置プロパティ
        private double _windowLeft;

        /// <summary>
        /// Windowの横軸の位置プロパティ
        /// </summary>
        public double WindowLeft
        {
            get => this._windowLeft;
            set => this.SetProperty(ref this._windowLeft, value);
        }
        #endregion

        #region Windowの縦軸の位置プロパティ
        private double _windowTop;

        /// <summary>
        /// Windowの縦軸の位置プロパティ
        /// </summary>
        public double WindowTop
        {
            get => this._windowTop;
            set => this.SetProperty(ref this._windowTop, value);
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="regionManager">DIコンテナから注入されるRegionManager</param>
        public MainWindowViewModel(IRegionManager regionManager,
                                    Services.ITitlebarOperationService titlebarOperationService,
                                    Services.ISettingsService settingsService)
        {
            // 下記privateフィールドのコンストラクタ注入
            this._regionManager = regionManager;
            this._titlebarOperationService = titlebarOperationService;
            this._settingsService = settingsService;

            // 下記Commandの作成
            this.NavigateRootRegionCommand = new DelegateCommand<string>(this.navigateRootRegion);
            this.GoBackRootRegionCommand = new DelegateCommand(this.goBackRootRegion, this.canGoBackRootRegion);
            this.GoForwardRootRegionCommand = new DelegateCommand(this.goForwardRootRegion, this.canGoForwardRootRegion);
            this.ContnetChangedCommand = new DelegateCommand(this.contentChanged);
            this.MinimizeButtonClickedCommand = new DelegateCommand(this.minimizeButtonClicked);
            this.NormalizeOrMaximizeButtonClickedCommand = new DelegateCommand(this.normalizeOrMaximizeButtonClicked);
            this.ShutdownButtonClickedCommand = new DelegateCommand(this.shutdownButtonClickedCommand);

            // 下記起動時用の初期設定
            this.WindowHeight = this._settingsService.MainWindowHeight;
            this.WindowWidth = this._settingsService.MainWindowWidth;
            this.WindowLeft = this._settingsService.MainWindowLeft;
            this.WindowTop = this._settingsService.MainWindowTop;

            var isNormalState = this._settingsService.MainWindowIsNormalState;
            if (isNormalState)
            {
                this._titlebarOperationService.Normalize();
            }
            else
            {
                this._titlebarOperationService.Maximize();
            }
        }
    }
}