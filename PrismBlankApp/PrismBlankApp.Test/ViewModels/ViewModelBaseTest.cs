using Moq;
using Prism.Regions;
using PrismBlankApp.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;
using Common = PrismBlankApp.Test.Common;

namespace PrismBlankApp.Test.ViewModels
{
    public class ViewModelBaseTest
    {
        /*
         * [ViewModelBaseオブジェクトが取りうる状態]
         * - Created : インスタンス作成直後で本ViewModelに対応するViewがRegionに表示すらされていない状態
         * - Showing : 本ViewModelに対応するViewがRegionに表示されている状態
         * - Showed : 本ViewModelに対応するViewがRegionに表示されていたが現在他のViewが表示されている状態
         */

        #region テストダブル関連
        // 下記テストで使用するダミーオブジェクト
        Mock<IRegionNavigationService> _regionNavigationService;
        Mock<IRegionNavigationJournal> _regionNavigationJournal;
        NavigationContext _navigatinContext;

        // テストケース毎にダミーオブジェクトを初期化するためのコンストラクタ
        public ViewModelBaseTest()
        {
            this._regionNavigationService = new Mock<IRegionNavigationService>();
            this._regionNavigationJournal = new Mock<IRegionNavigationJournal>();

            this._regionNavigationService
                .Setup(rns => rns.Journal)
                .Returns(this._regionNavigationJournal.Object);

            this._navigatinContext = new NavigationContext(this._regionNavigationService.Object, null);
        }
        #endregion

        #region テスト用のViewModelBaseオブジェクトを生成するためのファクトリーメソッド
        // Created状態のViewModelBaseオブジェクトを生成するためのファクトリーメソッド
        private ViewModelBase createdStateInstanceFactory()
        {
            return new ViewModelBase();
        }

        // Showing状態のViewModelBaseオブジェクトを生成するためのファクトリーメソッド
        private ViewModelBase showingStateInstanceFactory()
        {
            var result = new ViewModelBase();
            result.OnNavigatedTo(this._navigatinContext);
            return result;
        }

        // Showed状態のViewModelBaseオブジェクトを生成するためのファクトリーメソッド
        private ViewModelBase showedStateInstanceFactory()
        {
            var result = new ViewModelBase();
            result.OnNavigatedTo(this._navigatinContext);
            result.OnNavigatedFrom(this._navigatinContext);
            return result;
        }
        #endregion

        #region KeepAliveプロパティのテスト
        [Fact]
        public void KeepAlive_Created_ReturnFalse()
        {
            var viewModelBase = this.createdStateInstanceFactory();
            var result = viewModelBase.KeepAlive;
            Assert.False(result);
        }
        #endregion

        #region IsNavigationTargetメソッドのテスト
        [Fact]
        public void IsNavigationTarget_Created_ReturnFalse()
        {
            var viewModelBase = this.createdStateInstanceFactory();
            var result = viewModelBase.IsNavigationTarget(this._navigatinContext);
            Assert.False(result);
        }
        #endregion

        #region _regionNavigationServiceフィールドのテスト
        [Fact]
        public void _regionNavigationService_Created_IsNull()
        {
            const string FIELD_NAME = "_regionNavigationService";
            var viewModelBase = this.createdStateInstanceFactory();
            var regionNavigationServiceField = Common.PrivateFieldAccesser.GetPrivateField(viewModelBase, FIELD_NAME);
            Assert.Null(regionNavigationServiceField);
        }

        [Fact]
        public void _regionNavigationService_Showing_EqualDummyRegionNavigationService()
        {
            const string FIELD_NAME = "_regionNavigationService";
            var viewModelBase = this.showingStateInstanceFactory();
            var regionNavigationServiceField = Common.PrivateFieldAccesser.GetPrivateField(viewModelBase, FIELD_NAME);
            var result = object.ReferenceEquals(regionNavigationServiceField, this._regionNavigationService.Object);
            Assert.True(result);
        }
        #endregion

        #region PersistInHistoryメソッドのテスト
        [Fact]
        public void PersistInHistory_Created_ReturnTrue()
        {
            var viewModelBase = this.createdStateInstanceFactory();
            var result = viewModelBase.PersistInHistory();
            Assert.True(result);
        }
        #endregion

        #region NavigateCommandコマンドのテスト
        [Fact]
        public void NavigateCommand_Showing_ExecuteRaiseCanExecuteChanged()
        {
            var result = false;
            var viewModelBase = this.showingStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            viewModelBase.NavigateCommand.CanExecuteChanged += onCanExecuteChanged;
            this._regionNavigationService.Raise(m => m.Navigated += null, new RegionNavigationEventArgs(this._navigatinContext));
            viewModelBase.NavigateCommand.CanExecuteChanged -= onCanExecuteChanged;
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
            Assert.True(result);
        }

        [Fact]
        public void NavigateCommand_Showed_DontExecuteRaiseCanExecuteChanged()
        {
            var result = false;
            var viewModelBase = this.showedStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            viewModelBase.NavigateCommand.CanExecuteChanged += onCanExecuteChanged;
            this._regionNavigationService.Raise(rng => rng.Navigated += null, new RegionNavigationEventArgs(this._navigatinContext));
            viewModelBase.NavigateCommand.CanExecuteChanged -= onCanExecuteChanged;
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
            Assert.False(result);
        }

        [Fact]
        public void NavigateCommand_Execute_Showing_ExecutedDummyNavigationServicesRequestNavigate()
        {
            const string DUMMY_NEXT_VIEW_NAME = nameof(DUMMY_NEXT_VIEW_NAME);
            var result = false;
            this._regionNavigationService.Setup(x => x.RequestNavigate(It.IsAny<Uri>(), It.IsAny<Action<NavigationResult>>())).Callback(() => { result = true; });
            var viewModelBase = this.showingStateInstanceFactory();
            viewModelBase.NavigateCommand.Execute(DUMMY_NEXT_VIEW_NAME);
            Assert.True(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void NavigateCommand_CanExecute_Created_ReturnFalse()
        {
            const string DUMMY_NEXT_VIEW_NAME = nameof(DUMMY_NEXT_VIEW_NAME);
            var viewModelBase = this.createdStateInstanceFactory();
            var result = viewModelBase.NavigateCommand.CanExecute(DUMMY_NEXT_VIEW_NAME);
            Assert.False(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void NavigateCommand_CanExecute_Showing_ReturnTrue()
        {
            const string DUMMY_NEXT_VIEW_NAME = nameof(DUMMY_NEXT_VIEW_NAME);
            var viewModelBase = this.showingStateInstanceFactory();
            var result = viewModelBase.NavigateCommand.CanExecute(DUMMY_NEXT_VIEW_NAME);
            Assert.True(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }
        #endregion

        #region GoBackCommandコマンドのテスト
        [Fact]
        public void GoBackCommand_Showing_ExecuteRaiseCanExecuteChanged()
        {
            var result = false;
            var viewModelBase = this.showingStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            viewModelBase.GoBackCommand.CanExecuteChanged += onCanExecuteChanged;
            this._regionNavigationService.Raise(rng => rng.Navigated += null, new RegionNavigationEventArgs(this._navigatinContext));
            viewModelBase.GoBackCommand.CanExecuteChanged -= onCanExecuteChanged;
            Assert.True(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void GoBackCommand_Showed_DontExecuteRaiseCanExecuteChanged()
        {
            var result = false;
            var viewModelBase = this.showedStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            viewModelBase.GoBackCommand.CanExecuteChanged += onCanExecuteChanged;
            this._regionNavigationService.Raise(rng => rng.Navigated += null, new RegionNavigationEventArgs(this._navigatinContext));
            viewModelBase.GoBackCommand.CanExecuteChanged -= onCanExecuteChanged;
            Assert.False(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void GoBackCommand_Execute_Showing_ExecutedDummyNavigationJournalsGoBack()
        {
            var viewModelBase = this.showingStateInstanceFactory();
            viewModelBase.GoBackCommand.Execute();
            this._regionNavigationJournal.Verify(rnj => rnj.GoBack());
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void GoBackCommand_CanExecute_Created_ReturnFalse()
        {
            var viewModelBase = this.createdStateInstanceFactory();
            var result = viewModelBase.GoBackCommand.CanExecute();
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
            Assert.False(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GoBackCommand_CanExecute_Showing_ReturnValueEqualDummyNavigationServicesCanGoBack(bool canGoBackResult)
        {
            var viewModelBase = this.showingStateInstanceFactory();
            this._regionNavigationJournal.Setup(rnj => rnj.CanGoBack).Returns(canGoBackResult);
            var result = viewModelBase.GoBackCommand.CanExecute();
            Assert.Equal(canGoBackResult, result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }
        #endregion

        #region GoForwardCommandコマンドのテスト
        [Fact]
        public void GoForwardCommand_Showing_ExecuteRaiseCanExecuteChanged()
        {
            var result = false;
            var viewModelBase = this.showingStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            viewModelBase.GoForwardCommand.CanExecuteChanged += onCanExecuteChanged;
            this._regionNavigationService.Raise(rng => rng.Navigated += null, new RegionNavigationEventArgs(this._navigatinContext));
            viewModelBase.GoForwardCommand.CanExecuteChanged -= onCanExecuteChanged;
            Assert.True(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void GoForwardCommand_Showed_DontExecuteRaiseCanExecuteChanged()
        {
            var result = false;
            var viewModelBase = this.showedStateInstanceFactory();

            EventHandler onCanExecuteChanged = (sender, e) =>
            {
                result = true;
            };

            viewModelBase.GoForwardCommand.CanExecuteChanged += onCanExecuteChanged;
            this._regionNavigationService.Raise(rng => rng.Navigated += null, new RegionNavigationEventArgs(this._navigatinContext));
            viewModelBase.GoForwardCommand.CanExecuteChanged -= onCanExecuteChanged;
            Assert.False(result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void GoForwardCommand_Execute_Showing_ExecutedDummyNavigationJournalsGoForward()
        {
            var viewModelBase = this.showingStateInstanceFactory();
            viewModelBase.GoForwardCommand.Execute();
            this._regionNavigationJournal.Verify(rnj => rnj.GoForward());
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }

        [Fact]
        public void GoForwardCommand_CanExecute_Created_ReturnFalse()
        {
            var viewModelBase = this.createdStateInstanceFactory();
            var result = viewModelBase.GoForwardCommand.CanExecute();
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
            Assert.False(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GoForwardCommand_CanExecute_Showing_ReturnValueEqualDummyNavigationServicesCanGoForward(bool canGoBackResult)
        {
            var viewModelBase = this.showingStateInstanceFactory();
            this._regionNavigationJournal.Setup(rnj => rnj.CanGoForward).Returns(canGoBackResult);
            var result = viewModelBase.GoForwardCommand.CanExecute();
            Assert.Equal(canGoBackResult, result);
            viewModelBase.OnNavigatedFrom(this._navigatinContext);
        }
        #endregion
    }
}