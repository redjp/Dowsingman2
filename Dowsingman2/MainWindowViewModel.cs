using Dowsingman2.BaseClass;
using Dowsingman2.Dialog;
using Dowsingman2.LiveService;
using Dowsingman2.SubManager;
using Dowsingman2.UtilityClass;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace Dowsingman2
{
    public class MainWindowViewModel : BindableBase
    {
        #region -Field
        private ObservableCollection<LeftMenu> leftMenus_;
        private bool topMenuFavorite_;
        private bool topMenuAll_;

        private LeftMenu selectedLeftMenu_;
        private TopMenuSelection selectedTopMenu_;

        private Visibility buttonVisibility_;

        private ReadOnlyCollection<StreamClass> dataGridCollection_;
        private StreamClass selectedData_;
        #endregion

        #region -Property
        public ObservableCollection<LeftMenu> LeftMenus
        {
            get { return leftMenus_; }
            set
            {
                if (leftMenus_ == value) return;
                leftMenus_ = value;
                OnPropertyChanged();
            }
        }

        public bool TopMenuFavorite
        {
            get { return topMenuFavorite_; }
            set
            {
                if (topMenuFavorite_ == value) return;
                topMenuFavorite_ = value;
                OnPropertyChanged();
            }
        }

        public bool TopMenuAll
        {
            get { return topMenuAll_; }
            set
            {
                if (topMenuAll_ == value) return;
                topMenuAll_ = value;
                OnPropertyChanged();
            }
        }

        public LeftMenu SelectedLeftMenu
        {
            get { return selectedLeftMenu_; }
            set {
                if (selectedLeftMenu_ == value) return;
                selectedLeftMenu_ = value;
                OnPropertyChanged();
            }
        }

        public TopMenuSelection SelectedTopMenu
        {
            get { return selectedTopMenu_; }
            set {
                if (selectedTopMenu_ == value) return;
                selectedTopMenu_ = value;
                OnPropertyChanged();
            }
        }

        public Visibility ButtonVisibility
        {
            get { return buttonVisibility_; }
            set {
                if (buttonVisibility_ == value) return;
                buttonVisibility_ = value;
                OnPropertyChanged();
            }
        }

        public ReadOnlyCollection<StreamClass> DataGridCollection
        {
            get { return dataGridCollection_; }
            set {
                if (dataGridCollection_ == value) return;
                dataGridCollection_ = value;
                OnPropertyChanged();
            }
        }

        public StreamClass SelectedData
        {
            get { return selectedData_; }
            set
            {
                if (selectedData_ == value) return;
                selectedData_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region -Command
        public ICommand AddButtonCommand => new AnotherCommandImplementation(ExcuteOpenAddButtonDialog, CanExcuteOpenAddButtonDialog);
        public ICommand DeleteButtonCommand => new AnotherCommandImplementation(ExcuteOpenDeleteButtonDialog, CanExcuteOpenDeleteButtonDialog);
        public ICommand SettingButtonCommand => new AnotherCommandImplementation(ExcuteOpenSettingWindow);

        public ICommand LeftMenuCommand => new AnotherCommandImplementation(ExcuteLeftMenuChecked);
        public ICommand TopMenuCommand => new AnotherCommandImplementation(ExcuteTopMenuChecked);

        public ICommand DoubleClickCommand => new AnotherCommandImplementation(ExcuteOpenBrowser);
        #endregion

        #region -Constructor
        public MainWindowViewModel()
        {
            LeftMenus = CreateData();

            SettingManager.GetInstance().RecoverViewModel(this);
            ButtonVisibility = SettingManager.GetInstance().GetButtonVisibility();

            //一覧の更新通知を受け取る
            NotifyIconWrapper.RefreshCompleted += NotifyIconWrapper_RefreshCompleted;

            RefreshDataGrid();
        }
        #endregion

        /// <summary>
        /// 一覧の更新があった場合グリッドを更新
        /// </summary>
        private void NotifyIconWrapper_RefreshCompleted(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        #region --AddButtonCommand
        private bool CanExcuteOpenAddButtonDialog(object o)
        {
            if (!Equals(SelectedLeftMenu?.TopMenuVisibility, Visibility.Visible)) return false;
            return SelectedTopMenu == TopMenuSelection.Favorite || o != null;
        }

        private async void ExcuteOpenAddButtonDialog(object param)
        {
            var view = new AddButtonDialog();
            view.AddButtonTextBlock.Text = "追加する名前を入力 (" + SelectedLeftMenu.Text + ")" ;
            if (SelectedTopMenu == TopMenuSelection.All) view.AddButtonTextBox.Text = (param as StreamClass).Owner;

            var manager = SelectedLeftMenu.Manager;
            bool isTwitch = SelectedLeftMenu.Text == "Twitch";

            while (true)
            {
                var result = await DialogHost.Show(view);

                if (!Equals(result, true)) return;

                //追加
                string s = view.AddButtonTextBox.Text;
                if (string.IsNullOrWhiteSpace(s))
                {
                    //名前が入力されていません
                    var notify = new NotifyDialog();
                    notify.NotifyTextBlock.Text = "名前が入力されていません";
                    await DialogHost.Show(notify);
                    continue;
                }

                if (isTwitch && !s.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_'))
                {
                    //Twitchは英数小文字で入力してください
                    var notify = new NotifyDialog();
                    notify.NotifyTextBlock.Text = "Twitchは英数小文字で入力してください";
                    await DialogHost.Show(notify);
                    continue;
                }

                if (!manager.AddFavorite(s))
                {
                    //sはすでに登録されています
                    var notify = new NotifyDialog();
                    notify.NotifyTextBlock.Text = s + "はすでに登録されています";
                    await DialogHost.Show(notify);
                    return;
                }

                manager.Save();

                //Gridの更新
                RefreshBridge.Refresh(manager);
                RefreshDataGrid();

                break;
            }
        }
        #endregion

        #region --DeleteButtonCommand
        private bool CanExcuteOpenDeleteButtonDialog(object o)
        {
            if (o == null) return false;
            return Equals(SelectedLeftMenu?.TopMenuVisibility, Visibility.Visible) && SelectedTopMenu == TopMenuSelection.Favorite;
        }

        private async void ExcuteOpenDeleteButtonDialog(object param)
        {
            var view = new DeleteButtonDialog();
            string owner = (param as StreamClass).Owner;
            view.DeleteButtonTextBlock.Text = "\"" + owner + "\"を削除しますか";

            var manager = SelectedLeftMenu.Manager;

            var result = await DialogHost.Show(view);

            if (!Equals(result, true)) return;

            //削除
            if(!manager.RemoveFavorite(owner))
            {
                //削除に失敗しました
                var notify = new NotifyDialog();
                notify.NotifyTextBlock.Text = "削除に失敗しました";
                await DialogHost.Show(notify);
                return;
            }

            manager.Save();

            //Gridの更新
            RefreshBridge.Refresh(manager);
            RefreshDataGrid();
        }
        #endregion

        #region --SettingButtonCommand
        private void ExcuteOpenSettingWindow(object param)
        {
            if (param == null) return;

            var view = new SettingWindow()
            {
                DataContext = new SettingWindowViewModel()
            };
            view.Owner = param as Window;
            view.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            view.ShowDialog();

            ButtonVisibility = SettingManager.GetInstance().GetButtonVisibility();
        }
        #endregion

        #region --LeftMenuCommand
        private void ExcuteLeftMenuChecked(object param)
        {
            var value = param as LeftMenu;
            if (SelectedLeftMenu == value) return;
            SelectedLeftMenu = value;

            SettingManager.GetInstance().SaveViewModel(this);

            RefreshDataGrid();
        }
        #endregion

        #region --TopMenuCommand
        private void ExcuteTopMenuChecked(object param)
        {
            var value = param == null ? TopMenuSelection.Favorite : (TopMenuSelection)param;
            if (SelectedTopMenu == value) return;
            SelectedTopMenu = value;

            SettingManager.GetInstance().SaveViewModel(this);

            RefreshDataGrid();
        }
        #endregion

        #region --DoubleClickCommand
        private bool CanExcuteOpenBrowser(object o)
        {
            return o != null;
        }
        private void ExcuteOpenBrowser(object param)
        {
            var sc = param as StreamClass;
            MyUtility.OpenBrowser(sc?.Url);
        }
        #endregion

        /// <summary>
        /// 選択中のメニューに合わせてデータグリッドを更新
        /// </summary>
        private void RefreshDataGrid()
        {
            SelectedData = null;
            DataGridCollection = GetSelectedGridData(SelectedLeftMenu, SelectedTopMenu);
        }

        /// <summary>
        /// 選択中のグリッドのデータを取得
        /// </summary>
        private ReadOnlyCollection<StreamClass> GetSelectedGridData(LeftMenu leftMenu, TopMenuSelection topMenu)
        {
            if (leftMenu.Manager == null) return GetFavoriteAll();

            if (leftMenu.TopMenuVisibility == Visibility.Hidden) return leftMenu.Manager.GetFavoriteStreamClassList();

            if (topMenu == TopMenuSelection.Favorite) return leftMenu.Manager.GetFavoriteStreamClassList();
            
            return leftMenu.Manager.GetLiveStreamClassList();
        }

        /// <summary>
        /// 配信中のお気に入り一覧を取得
        /// </summary>
        private ReadOnlyCollection<StreamClass> GetFavoriteAll()
        {
            var result = new List<StreamClass>();
            foreach (AbstractManager manager in LeftMenus.Where(x => x.TopMenuVisibility == Visibility.Visible).Select(x => x.Manager))
            {
                result.AddRange(manager.GetFavoriteLiveOnly());
            }
            return result.AsReadOnly();
        }

        private static ObservableCollection<LeftMenu> CreateData()
        {
            return new ObservableCollection<LeftMenu>
            {
                new LeftMenu
                {
                    IsSelected = true,
                    TopMenuVisibility = Visibility.Hidden,
                    IconKind = PackIconKind.StarFace,
                    Text = "Favorite",
                    Manager = null
                },
                new LeftMenu
                {
                    IsSelected = false,
                    TopMenuVisibility = Visibility.Visible,
                    IconKind = PackIconKind.AlphaKBox,
                    Text = "Kukulu",
                    Manager = KukuluManager.GetInstance()
                },
                new LeftMenu
                {
                    IsSelected = false,
                    TopMenuVisibility = Visibility.Visible,
                    IconKind = PackIconKind.AlphaTBox,
                    Text = "Twitch",
                    Manager = TwitchManager.GetInstance()
                },
                new LeftMenu
                {
                    IsSelected = false,
                    TopMenuVisibility = Visibility.Visible,
                    IconKind = PackIconKind.AlphaFBox,
                    Text = "Fc2",
                    Manager = Fc2Manager.GetInstance()
                },
                new LeftMenu
                {
                    IsSelected = false,
                    TopMenuVisibility = Visibility.Visible,
                    IconKind = PackIconKind.AlphaCBox,
                    Text = "Cavetube",
                    Manager = CavetubeManager.GetInstance()
                },
                new LeftMenu
                {
                    IsSelected = false,
                    TopMenuVisibility = Visibility.Hidden,
                    IconKind = PackIconKind.ClipboardOutline,
                    Text = "Log",
                    Manager = LogManager.GetInstance()
                }
            };
        }
    }
}
