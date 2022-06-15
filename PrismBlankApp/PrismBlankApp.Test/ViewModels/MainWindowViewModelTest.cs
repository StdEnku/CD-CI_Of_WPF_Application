using Moq;
using Prism.Regions;
using PrismBlankApp.Services;
using PrismBlankApp.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace PrismBlankApp.Test.ViewModels
{
    public class MainWindowViewModelTest
    {
        /*
         * [MainWindowViewModelの取りうる状態]
         * - Created : インスタンスが作成された直後の状態
         * - WindowNormalState : Windowのサイズが普通の状態
         * - WindowMaximizeState : Windowのサイズが最大化されてる状態
         * - SavedWindowNormalState : 前回WindowStateがNormalの状態で終了して起動した状態
         * - SavedWindowMaximizeState : 前回WindowStateがMaximizzeの状態で終了して起動した状態
         */

        #region テストダブル関連
        private Mock<IRegionManager> _regionManager;
        private Mock<IRegionCollection> _regionCollection;
        private Mock<IRegion> _region;
        private Mock<IRegionNavigationService> _regionNavigationService;
        private Mock<IRegionNavigationJournal> _regionNavigationJournal;
        private Mock<ITitlebarOperationService> _titlebarOperationService;
        private Mock<ISettingsService> _settingsService;

        public MainWindowViewModelTest()
        {
            this._regionManager = new Mock<IRegionManager>();
            this._regionCollection = new Mock<IRegionCollection>();
            this._region = new Mock<IRegion>();
            this._regionNavigationService = new Mock<IRegionNavigationService>();
            this._regionNavigationJournal = new Mock<IRegionNavigationJournal>();
            this._titlebarOperationService = new Mock<ITitlebarOperationService>();
            this._settingsService = new Mock<ISettingsService>();

            this._regionManager.Setup(rm => rm.Regions).Returns(this._regionCollection.Object);
            this._regionCollection.Setup(rc => rc[Constants.RegionNames.RootRegion]).Returns(this._region.Object);
            this._region.Setup(r => r.NavigationService).Returns(this._regionNavigationService.Object);
            this._regionNavigationService.Setup(rns => rns.Journal).Returns(this._regionNavigationJournal.Object);
            this._regionManager.Setup(rm => rm.Regions.ContainsRegionWithName(Constants.RegionNames.RootRegion)).Returns(true);
        }
        #endregion

        #region テスト用のMainWindowViewModelオブジェクトを生成するためのファクトリーメソッド
        private MainWindowViewModel createdStateInstanceFactory()
        {
            return new MainWindowViewModel(this._regionManager.Object, this._titlebarOperationService.Object, this._settingsService.Object);
        }

        private MainWindowViewModel windowNormalStateInstanceFactory()
        {
            this._titlebarOperationService.Setup(tos => tos.IsNormalState()).Returns(true);
            return new MainWindowViewModel(this._regionManager.Object, this._titlebarOperationService.Object, this._settingsService.Object);
        }

        private MainWindowViewModel windowMaximizeStateInstanceFactory()
        {
            this._titlebarOperationService.Setup(tos => tos.IsNormalState()).Returns(false);
            return new MainWindowViewModel(this._regionManager.Object, this._titlebarOperationService.Object, this._settingsService.Object);
        }

        private MainWindowViewModel savedWindowNormalStateInstanceFactory()
        {
            this._settingsService.Setup(ss => ss.MainWindowIsNormalState).Returns(true);
            return new MainWindowViewModel(this._regionManager.Object, this._titlebarOperationService.Object, this._settingsService.Object);
        }

        private MainWindowViewModel savedWindowMaximizeStateInstanceFactory()
        {
            this._settingsService.Setup(ss => ss.MainWindowIsNormalState).Returns(false);
            return new MainWindowViewModel(this._regionManager.Object, this._titlebarOperationService.Object, this._settingsService.Object);
        }
        #endregion

        #region NavigateRootRegionCommandコマンドのテスト
        [Fact]
        public void NavigateRootRegionCommand_Execute_Created_ExecuteDummyRegionManagersRequestNavigate()
        {
            const string DUMMY_NEXT_VIEW_NAME = nameof(DUMMY_NEXT_VIEW_NAME);
            var mainWindowViewModel = this.createdStateInstanceFactory();
            mainWindowViewModel.NavigateRootRegionCommand.Execute(DUMMY_NEXT_VIEW_NAME);
            this._regionManager.Verify(rm => rm.RequestNavigate(Constants.RegionNames.RootRegion, DUMMY_NEXT_VIEW_NAME));
        }
        #endregion

        #region GoBackRootRegionCommandコマンドのテスト
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GoBackRootRegionCommand_CanExecute_Created_ResultEqualDummyNavigationJournalsCanGoBack(bool canGoBack)
        {
            var mainWindowViewModel = this.createdStateInstanceFactory();
            this._regionNavigationJournal.Setup(rnj => rnj.CanGoBack).Returns(canGoBack);
            var result = mainWindowViewModel.GoBackRootRegionCommand.CanExecute();
            Assert.Equal(result, canGoBack);
        }

        [Fact]
        public void GoBackRootRegionCommand_Execute_Created_ExecuteDummyRegionNavigationJournalsGoBack()
        {
            var mainWindowViewModel = this.createdStateInstanceFactory();
            mainWindowViewModel.GoBackRootRegionCommand.Execute();
            this._regionNavigationJournal.Verify(rnj => rnj.GoBack());
        }
        #endregion

        #region GoForwardRootRegionCommandコマンドのテスト
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GoForwardRootRegionCommand_CanExecute_Created_ResultEqualDummyNavigationJournalsCanGoForward(bool canGoForward)
        {
            var mainWindowViewModel = this.createdStateInstanceFactory();
            this._regionNavigationJournal.Setup(rnj => rnj.CanGoForward).Returns(canGoForward);
            var result = mainWindowViewModel.GoForwardRootRegionCommand.CanExecute();
            Assert.Equal(result, canGoForward);
        }

        [Fact]
        public void GoForwardRootRegionCommand_Execute_Created_ExecuteDummyRegionNavigationJournalsGoForward()
        {
            var mainWindowViewModel = this.createdStateInstanceFactory();
            mainWindowViewModel.GoForwardRootRegionCommand.Execute();
            this._regionNavigationJournal.Verify(rnj => rnj.GoForward());
        }
        #endregion

        #region ContnetChangedCommandコマンドのテスト
        [Fact]
        public void ContnetChangedCommand_Created_ExecuteGoBackRootRegionCommandsRaiseCanExecuteChanged()
        {
            var result = false;
            var mainWindowViewModel = createdStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            mainWindowViewModel.GoBackRootRegionCommand.CanExecuteChanged += onCanExecuteChanged;
            mainWindowViewModel.ContnetChangedCommand.Execute();
            mainWindowViewModel.GoBackRootRegionCommand.CanExecuteChanged -= onCanExecuteChanged;
            Assert.True(result);
        }

        [Fact]
        public void ContnetChangedCommand_Created_ExecuteGoForwardRootRegionCommandsRaiseCanExecuteChanged()
        {
            var result = false;
            var mainWindowViewModel = createdStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            mainWindowViewModel.GoForwardRootRegionCommand.CanExecuteChanged += onCanExecuteChanged;
            mainWindowViewModel.ContnetChangedCommand.Execute();
            mainWindowViewModel.GoForwardRootRegionCommand.CanExecuteChanged -= onCanExecuteChanged;
            Assert.True(result);
        }
        #endregion

        #region MinimizeButtonClickedCommandコマンドのテスト
        [Fact]
        public void MinimizeButtonClickedCommand_Created_ExecuteTitlebarOperationServicesMinimize()
        {
            var mainWindowViewModel = this.createdStateInstanceFactory();
            mainWindowViewModel.MinimizeButtonClickedCommand.Execute();
            this._titlebarOperationService.Verify(tos => tos.Minimize());
        }
        #endregion

        #region NormalizeOrMaximizeButtonClickedCommandコマンドのテスト
        [Fact]
        public void NormalizeOrMaximizeButtonClickedCommand_WindowNormalState_ExecuteTitlebarOperationServicesMaximize()
        {
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.NormalizeOrMaximizeButtonClickedCommand.Execute();
            this._titlebarOperationService.Verify(tos => tos.Maximize());
        }

        [Fact]
        public void NormalizeOrMaximizeButtonClickedCommand_WindowMaximizeState_ExecuteTitlebarOperationServicesNormalize()
        {
            var mainWindowViewModel = this.windowMaximizeStateInstanceFactory();
            mainWindowViewModel.NormalizeOrMaximizeButtonClickedCommand.Execute();
            this._titlebarOperationService.Verify(tos => tos.Normalize());
        }
        #endregion

        #region ShutdownButtonClickedCommandコマンドのテスト
        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_SetSettingServicesMainWindowHeightFromWindowHeightProperty()
        {
            const double HEIGHT = 100;
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.WindowHeight = HEIGHT;
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.VerifySet(ss => ss.MainWindowHeight = mainWindowViewModel.WindowHeight);
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_SetSettingServicesMainWindowWidthFromWindowWidthProperty()
        {
            const double WIDTH = 100;
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.WindowWidth = WIDTH;
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.VerifySet(ss => ss.MainWindowWidth = mainWindowViewModel.WindowWidth);
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_SetSettingServicesMainWindowLeftFromWindowLeftProperty()
        {
            const double LEFT = 100;
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.WindowLeft = LEFT;
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.VerifySet(ss => ss.MainWindowLeft = mainWindowViewModel.WindowLeft);
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_SetSettingServicesMainWindowTopFromWindowTopProperty()
        {
            const double TOP = 100;
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.WindowTop = TOP;
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.VerifySet(ss => ss.MainWindowTop = mainWindowViewModel.WindowTop);
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_SetSettingServicesMainWindowIsNormalStateTrue()
        {
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.VerifySet(ss => ss.MainWindowIsNormalState = true);
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_ExecuteSettingServicesSaveMethod()
        {
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.Verify(ss => ss.Save());
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowNormalState_ExecuteTitlebarOperationServicesShutdownMethod()
        {
            var mainWindowViewModel = this.windowNormalStateInstanceFactory();
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._titlebarOperationService.Verify(tos => tos.Shutdown());
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowMaximizeState_SetSettingServicesMainWindowIsNormalStateFalse()
        {
            var mainWindowViewModel = this.windowMaximizeStateInstanceFactory();
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.VerifySet(ss => ss.MainWindowIsNormalState = false);
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowMaximizeState_ExecuteSettingServicesSaveMethod()
        {
            var mainWindowViewModel = this.windowMaximizeStateInstanceFactory();
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._settingsService.Verify(ss => ss.Save());
        }

        [Fact]
        public void ShutdownButtonClickedCommand_WindowMaximizeState_ExecuteTitlebarOperationServicesShutdownMethod()
        {
            var mainWindowViewModel = this.windowMaximizeStateInstanceFactory();
            mainWindowViewModel.ShutdownButtonClickedCommand.Execute();
            this._titlebarOperationService.Verify(tos => tos.Shutdown());
        }
        #endregion

        #region コンストラクタのテスト
        [Fact]
        public void Constructor_SavedWindowNormalState_ExecuteTitlebarOperationServicesNormalizeMethod()
        {
            var mainWindowViewModel = this.savedWindowNormalStateInstanceFactory();
            this._titlebarOperationService.Verify(tos => tos.Normalize());
        }

        [Fact]
        public void Constructor_SavedWindowMaximizeState_ExecuteTitlebarOperationServicesMaximizeMethod()
        {
            var mainWindowViewModel = this.savedWindowMaximizeStateInstanceFactory();
            this._titlebarOperationService.Verify(tos => tos.Maximize());
        }

        [Fact]
        public void Constructor_Created_EqualSettingServicesMainWindowHeightAndMainWindowViewModelsWindowHeightAreEqual()
        {
            const double HEIGHT = 100;
            this._settingsService.Setup(ss => ss.MainWindowHeight).Returns(HEIGHT);
            var mainWindowViewModel = this.createdStateInstanceFactory();

            var settingsServicesMainWindowHeight = this._settingsService.Object.MainWindowHeight;
            var mainWindowViewModelsWindowHeight = mainWindowViewModel.WindowHeight;
            Assert.Equal(settingsServicesMainWindowHeight, mainWindowViewModelsWindowHeight);
        }

        [Fact]
        public void Constructor_Created_EqualSettingServicesMainWindowWidthAndMainWindowViewModelsWindowWidthAreEqual()
        {
            const double WIDTH = 100;
            this._settingsService.Setup(ss => ss.MainWindowWidth).Returns(WIDTH);
            var mainWindowViewModel = this.createdStateInstanceFactory();

            var settingsServicesMainWindowWidth = this._settingsService.Object.MainWindowWidth;
            var mainWindowViewModelsWindowWidth = mainWindowViewModel.WindowWidth;
            Assert.Equal(settingsServicesMainWindowWidth, mainWindowViewModelsWindowWidth);
        }

        [Fact]
        public void Constructor_Created_EqualSettingServicesMainWindowLeftAndMainWindowViewModelsWindowLeftAreEqual()
        {
            const double LEFT = 100;
            this._settingsService.Setup(ss => ss.MainWindowWidth).Returns(LEFT);
            var mainWindowViewModel = this.createdStateInstanceFactory();

            var settingsServicesMainWindowLeft = this._settingsService.Object.MainWindowLeft;
            var mainWindowViewModelsWindowLeft = mainWindowViewModel.WindowLeft;
            Assert.Equal(settingsServicesMainWindowLeft, mainWindowViewModelsWindowLeft);
        }

        [Fact]
        public void Constructor_Created_EqualSettingServicesMainWindowTopAndMainWindowViewModelsWindowTopAreEqual()
        {
            const double TOP = 100;
            this._settingsService.Setup(ss => ss.MainWindowWidth).Returns(TOP);
            var mainWindowViewModel = this.createdStateInstanceFactory();

            var settingsServicesMainWindowTop = this._settingsService.Object.MainWindowTop;
            var mainWindowViewModelsWindowTop = mainWindowViewModel.WindowTop;
            Assert.Equal(settingsServicesMainWindowTop, mainWindowViewModelsWindowTop);
        }
        #endregion
    }
}