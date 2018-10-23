using Dowsingman2.BaseClass;
using Dowsingman2.LiveService;
using Dowsingman2.Properties;
using Dowsingman2.SubManager;
using System.ComponentModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace Dowsingman2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public int LastTabIndex { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // ウィンドウのサイズを復元
            RecoverWindowBounds();

            NotifyIconWrapper.UpdateCompleted += NotifyIconWrapper_UpdateCompleted;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // ウィンドウのサイズを保存
            SaveWindowBounds();
            
            NotifyIconWrapper.UpdateCompleted -= NotifyIconWrapper_UpdateCompleted;
        }

        public void NotifyIconWrapper_UpdateCompleted(object sender, EventArgs e)
        {
            UpdateDispList();
        }

        /// <summary>
        /// 選択中のタブ名を取得する
        /// </summary>
        /// <returns></returns>
        private string GetSelectedTab()
        {
            switch (TabControl1.SelectedIndex)
            {
                case 0:
                    return "kukuluGrid";
                case 1:
                    return "kukuluGrid2";
                case 2:
                    return "fc2Grid";
                case 3:
                    return "fc2Grid2";
                case 4:
                    return "twitchGrid";
                case 5:
                    return "twitchGrid2";
                case 6:
                    return "cavetubeGrid";
                case 7:
                    return "cavetubeGrid2";
                case 8:
                    return "logGrid";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 対象グリッドをソート
        /// https://docs.microsoft.com/ja-jp/dotnet/framework/wpf/controls/how-to-group-sort-and-filter-data-in-the-datagrid-control
        /// </summary>
        /// <param name="targetGrid">並べ替え対象グリッド</param>
        private void SortList(DataGrid targetGrid)
        {
            ICollectionView cvTasks = CollectionViewSource.GetDefaultView(targetGrid.ItemsSource);
            if (cvTasks != null && cvTasks.CanSort == true)
            {
                cvTasks.SortDescriptions.Clear();
                cvTasks.SortDescriptions.Add(new SortDescription("Start_Time", ListSortDirection.Descending));
            }
        }

        /// <summary>
        /// 開いているGridの更新
        /// 参考URL
        /// http://oita.oika.me/2014/11/03/wpf-datagrid-binding/
        /// </summary>
        public void UpdateDispList()
        {
            DataGrid grid = null;
            AbstractManager manager = null;
            bool isFavorite = true;

            switch (GetSelectedTab())
            {
                case "kukuluGrid":
                    grid = kukuluGrid;
                    manager = KukuluManager.GetInstance();
                    isFavorite = true;
                    break;
                case "kukuluGrid2":
                    grid = kukuluGrid2;
                    manager = KukuluManager.GetInstance();
                    isFavorite = false;
                    break;
                case "fc2Grid":
                    grid = fc2Grid;
                    manager = Fc2Manager.GetInstance();
                    isFavorite = true;
                    break;
                case "fc2Grid2":
                    grid = fc2Grid2;
                    manager = Fc2Manager.GetInstance();
                    isFavorite = false;
                    break;
                case "twitchGrid":
                    grid = twitchGrid;
                    manager = TwitchManager.GetInstance();
                    isFavorite = true;
                    break;
                case "twitchGrid2":
                    grid = twitchGrid2;
                    manager = TwitchManager.GetInstance();
                    isFavorite = false;
                    break;
                case "cavetubeGrid":
                    grid = cavetubeGrid;
                    manager = CavetubeManager.GetInstance();
                    isFavorite = true;
                    break;
                case "cavetubeGrid2":
                    grid = cavetubeGrid2;
                    manager = CavetubeManager.GetInstance();
                    isFavorite = false;
                    break;
                case "logGrid":
                    grid = logGrid;
                    manager = LogManager.GetInstance();
                    isFavorite = true;
                    break;
            }
            if (grid != null && manager != null)
            {
                if (isFavorite)
                    grid.ItemsSource = manager.GetFavoriteStreamClassList();
                else
                    grid.ItemsSource = manager.GetLiveStreamClassList();

                SortList(grid);
            }
        }

        /// <summary>
        /// 新しい登録チャンネルの追加
        /// </summary>
        private void AddNewChannel()
        {
            if (Textbox1.Text != string.Empty)
            {
                AbstractManager manager = null;
                switch (GetSelectedTab())
                {
                    case "kukuluGrid":
                        manager = KukuluManager.GetInstance();
                        break;
                    case "fc2Grid":
                        manager = Fc2Manager.GetInstance();
                        break;

                    case "twitchGrid":
                        manager = TwitchManager.GetInstance();
                        break;

                    case "CavetubeGrid":
                        manager = CavetubeManager.GetInstance();
                        break;
                }
                if (manager != null && manager.AddFavorite(new StreamClass(Textbox1.Text)))
                {
                    MessageBox.Show(Textbox1.Text + "を追加しました。");
                    //内容を削除
                    Textbox1.Text = string.Empty;
                    UpdateDispList();
                    manager.Save();
                }
            }
        }

        /// <summary>
        /// 登録チャンネルの削除
        /// </summary>
        private void DeleteChannel()
        {
            DataGrid grid = null;
            AbstractManager manager = null;
            switch (GetSelectedTab())
            {
                case "kukuluGrid":
                    grid = kukuluGrid;
                    manager = KukuluManager.GetInstance();
                    break;
                case "fc2Grid":
                    grid = fc2Grid;
                    manager = Fc2Manager.GetInstance();
                    break;
                case "twitchGrid":
                    grid = twitchGrid;
                    manager = TwitchManager.GetInstance();
                    break;
                case "cavetubeGrid":
                    grid = cavetubeGrid;
                    manager = CavetubeManager.GetInstance();
                    break;
                case "logGrid":
                    grid = logGrid;
                    manager = LogManager.GetInstance();
                    break;
            }
            if (grid != null && manager != null && grid.SelectedIndex != -1)
            {
                StreamClass item = grid.SelectedItem as StreamClass;
                if (MessageBox.Show(item.Owner + "を削除しますか？", string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

                //選択されている項目を削除
                if (manager.RemoveFavorite(item))
                {
                    MessageBox.Show(item.Owner + "を削除しました");
                    UpdateDispList();
                    manager.Save();
                }
                else
                {
                    MessageBox.Show("削除に失敗しました");
                }
            }
        }

        /// <summary>
        /// 追加ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Button_add_Click(object sender, RoutedEventArgs e)
        {
            AddNewChannel();
        }

        /// <summary>
        /// 削除ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Button_del_Click(object sender, RoutedEventArgs e)
        {
            DeleteChannel();
        }

        /// <summary>
        /// テキストボックスでキー入力があった場合の処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Textbox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //押されたキーがEnterなら
            if (e.Key == Key.Return)
            {
                AddNewChannel();
            }
        }

        /// <summary>
        /// データグリッドでキー入力があった場合の処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //押されたキーがDeleteなら
            if (e.Key == Key.Delete)
            {
                DeleteChannel();
            }
        }

        /// <summary>
        /// 一覧タブの内容がダブルクリックされたとき
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void kukuluGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (kukuluGrid2.SelectedIndex >= 0)
            {
                var item = (StreamClass)kukuluGrid2.SelectedItem;
                AddFavoriteFromGrid(KukuluManager.GetInstance(), item);
            }
        }
        private void fc2Grid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (fc2Grid2.SelectedIndex >= 0)
            {
                var item = (StreamClass)fc2Grid2.SelectedItem;
                AddFavoriteFromGrid(Fc2Manager.GetInstance(), item);
            }
        }
        private void twitchGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (twitchGrid2.SelectedIndex >= 0)
            {
                var item = (StreamClass)twitchGrid2.SelectedItem;
                AddFavoriteFromGrid(TwitchManager.GetInstance(), item);
            }
        }
        private void cavetubeGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (cavetubeGrid2.SelectedIndex >= 0)
            {
                var item = cavetubeGrid2.SelectedItem as StreamClass;
                AddFavoriteFromGrid(CavetubeManager.GetInstance(), item);
            }
        }

        private void AddFavoriteFromGrid(AbstractManager manager, StreamClass newFavorite)
        {
            if (manager.AddFavorite(newFavorite))
            {
                LogManager.GetInstance().AddFavorite(newFavorite);
                MessageBox.Show(newFavorite.Owner + "を追加しました。");
                UpdateDispList();
                manager.Save();
            }
        }

        /// <summary>
        /// タブの内容がダブルクリックされたとき選択URLを開く
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void kukuluGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridDoubleClicked(kukuluGrid);
        }
        private void fc2Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridDoubleClicked(fc2Grid);
        }
        private void twitchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridDoubleClicked(twitchGrid);
        }
        private void cavetubeGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridDoubleClicked(cavetubeGrid);
        }
        private void logGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridDoubleClicked(logGrid);
        }

        private void GridDoubleClicked(DataGrid grid)
        {
            //選択されている項目があるか
            if (grid.SelectedIndex >= 0)
            {
                var url = (grid.SelectedItem as StreamClass).Url;
                if (url != string.Empty)
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(url);
            }
        }

        //音量調整機能をつけるまでの応急処置
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var player = new System.Media.SoundPlayer(Path.GetFullPath(".\\resource\\favorite.wav"));
            player.Play();
        }

        /// <summary>
        /// タブ切り替え時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LastTabIndex != TabControl1.SelectedIndex)
            {
                LastTabIndex = TabControl1.SelectedIndex;
                UpdateDispList();
            }
        }

        /// <summary>
        /// ウィンドウの位置・サイズを保存します。
        /// </summary>
        void SaveWindowBounds()
        {
            var settings = Settings.Default;
            settings.WindowMaximized = WindowState == WindowState.Maximized;
            WindowState = WindowState.Normal; // 最大化解除
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.SelectedTabIndex = TabControl1.SelectedIndex;
            settings.Save();
        }

        /// <summary>
        /// ウィンドウの位置・サイズを復元します。
        /// </summary>
        private void RecoverWindowBounds()
        {
            var settings = Settings.Default;
            if (settings.WindowLeft >= 0 &&
                (settings.WindowLeft + settings.WindowWidth) < SystemParameters.VirtualScreenWidth)
            {
                Left = settings.WindowLeft;
            }
            if (settings.WindowTop >= 0 &&
                (settings.WindowTop + settings.WindowHeight) < SystemParameters.VirtualScreenHeight)
            {
                Top = settings.WindowTop;
            }
            if (settings.WindowWidth >= 600 &&
               settings.WindowWidth <= SystemParameters.WorkArea.Width)
            {
                Width = settings.WindowWidth;
            }
            if (settings.WindowHeight >= 360 &&
               settings.WindowHeight <= SystemParameters.WorkArea.Height)
            {
                Height = settings.WindowHeight;
            }
            if (settings.WindowMaximized)
            {
                // ロード後に最大化
                Loaded += (o, e) => WindowState = WindowState.Maximized;
            }
            if (settings.SelectedTabIndex >= 0)
            {
                TabControl1.SelectedIndex = settings.SelectedTabIndex;
            }
        }
    }
}
