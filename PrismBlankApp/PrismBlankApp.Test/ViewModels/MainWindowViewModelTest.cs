using Moq;
using Prism.Regions;
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
         */

        #region テストダブル関連
        private Mock<IRegionManager> _regionManager;
        private Mock<IRegionCollection> _regionCollection;
        private Mock<IRegion> _region;
        private Mock<IRegionNavigationService> _regionNavigationService;
        private Mock<IRegionNavigationJournal> _regionNavigationJournal;

        public MainWindowViewModelTest()
        {
            this._regionManager = new Mock<IRegionManager>();
            this._regionCollection = new Mock<IRegionCollection>();
            this._region = new Mock<IRegion>();
            this._regionNavigationService = new Mock<IRegionNavigationService>();
            this._regionNavigationJournal = new Mock<IRegionNavigationJournal>();

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
            return new MainWindowViewModel(this._regionManager.Object);
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
    }
}