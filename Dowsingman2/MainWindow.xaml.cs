using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dowsingman2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            UpdateDispList();
        }

        /// <summary>
        /// 選択中のタブ名を取得する
        /// </summary>
        /// <returns></returns>
        private string GetSelectedTab()
        {
            if (TabControl1.SelectedIndex == 0)
                return "kukuluGrid";
            if (TabControl1.SelectedIndex == 1)
                return "kukuluGrid2";
            if (TabControl1.SelectedIndex == 2)
                return "fc2Grid";
            if (TabControl1.SelectedIndex == 3)
                return "fc2Grid2";
            if (TabControl1.SelectedIndex == 4)
                return "twitchGrid";
            if (TabControl1.SelectedIndex == 5)
                return "twitchGrid2";
            if (TabControl1.SelectedIndex == 6)
                return "cavetubeGrid";
            if (TabControl1.SelectedIndex == 7)
                return "cavetubeGrid2";
            if (TabControl1.SelectedIndex == 8)
                return "logGrid";

            return null;
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
            if (Kukulu.EnableChange)
            {
                if (GetSelectedTab() == "kukuluGrid")
                {
                    kukuluGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(Kukulu.List);
                    SortList(kukuluGrid);
                    if (!Textbox1.IsFocused) kukuluGrid.Focus();
                }
                if (GetSelectedTab() == "kukuluGrid2")
                {
                    kukuluGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Kukulu.All);
                    SortList(kukuluGrid2);
                    if (!Textbox1.IsFocused) kukuluGrid2.Focus();
                }
            }
            if (Fc2.EnableChange)
            {
                if (GetSelectedTab() == "fc2Grid")
                {
                    fc2Grid.ItemsSource = new ReadOnlyCollection<StreamClass>(Fc2.List);
                    SortList(fc2Grid);
                    if (!Textbox1.IsFocused) fc2Grid.Focus();
                }
                if (GetSelectedTab() == "fc2Grid2")
                {
                    fc2Grid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Fc2.All);
                    SortList(fc2Grid2);
                    if (!Textbox1.IsFocused) fc2Grid2.Focus();
                }
            }
            if (Twitch.EnableChange)
            {
                if (GetSelectedTab() == "twitchGrid")
                {
                    twitchGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(Twitch.List);
                    SortList(twitchGrid);
                    if (!Textbox1.IsFocused) twitchGrid.Focus();
                }
                if (GetSelectedTab() == "twitchGrid2")
                {
                    twitchGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Twitch.All);
                    SortList(twitchGrid2);
                    if (!Textbox1.IsFocused) twitchGrid2.Focus();
                }
            }
            if (Cavetube.EnableChange)
            {
                if (GetSelectedTab() == "cavetubeGrid")
                {
                    cavetubeGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(Cavetube.List);
                    SortList(cavetubeGrid);
                    if (!Textbox1.IsFocused) cavetubeGrid.Focus();
                }
                if (GetSelectedTab() == "cavetubeGrid2")
                {
                    cavetubeGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Cavetube.All);
                    SortList(cavetubeGrid2);
                    if (!Textbox1.IsFocused) cavetubeGrid2.Focus();
                }
            }
            if (GetSelectedTab() == "logGrid")
            {
                logGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.logList);
                SortList(logGrid);
                if (!Textbox1.IsFocused) logGrid.Focus();
            }
        }

        /// <summary>
        /// 新しい登録チャンネルの追加
        /// </summary>
        private void AddNewChannel()
        {
            //選択中のタブがkukuluなら
            if (GetSelectedTab() == "kukuluGrid")
                if (Kukulu.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Kukulu.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != string.Empty)
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Kukulu.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = string.Empty;
                        App.SaveList("Kukulu");
                        UpdateDispList();
                    }

            //選択中のタブがFC2なら
            if (GetSelectedTab() == "fc2Grid")
                if (Fc2.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Fc2.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != string.Empty)
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Fc2.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = string.Empty;
                        App.SaveList("Fc2");
                        UpdateDispList();
                    }

            //選択中のタブがtwitchなら
            if (GetSelectedTab() == "twitchGrid")
                if (Twitch.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Twitch.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != string.Empty)
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Twitch.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = string.Empty;
                        App.SaveList("Twitch");
                        UpdateDispList();
                    }

            //選択中のタブがcavetubeなら
            if (GetSelectedTab() == "cavetubeGrid")
                if (Cavetube.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Cavetube.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != string.Empty)
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Cavetube.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = string.Empty;
                        App.SaveList("Cavetube");
                        UpdateDispList();
                    }
        }

        /// <summary>
        /// 登録チャンネルの削除
        /// </summary>
        private void DeleteChannel()
        {
            //選択中のタブがkukuluなら
            if (GetSelectedTab() == "kukuluGrid")
            {
                //選択されている項目があるか
                if (kukuluGrid.SelectedIndex != -1)
                {
                    StreamClass item = (StreamClass)kukuluGrid.SelectedItem;
                    if (MessageBox.Show(item.Owner + "を削除しますか？", string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        return;

                    if (Kukulu.EnableChange)

                        //選択されている項目を削除
                        Kukulu.List.Remove(item);
                    MessageBox.Show(item.Owner + "を削除しました");
                    App.SaveList("Kukulu");
                    UpdateDispList();
                }
            }

            //選択中のタブがFC2なら
            if (GetSelectedTab() == "fc2Grid")
            {
                //選択されている項目があるか
                if (fc2Grid.SelectedIndex != -1)
                {
                    StreamClass item = (StreamClass)fc2Grid.SelectedItem;
                    if (MessageBox.Show(item.Owner + "を削除しますか？", string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        return;

                    if (Fc2.EnableChange)

                        //選択されている項目を削除
                        Fc2.List.Remove(item);
                    MessageBox.Show(item.Owner + "を削除しました");
                    App.SaveList("Fc2");
                    UpdateDispList();
                }
            }

            //選択中のタブがtwtichなら
            if (GetSelectedTab() == "twitchGrid")
            {
                //選択されている項目があるか
                if (twitchGrid.SelectedIndex != -1)
                {
                    StreamClass item = (StreamClass)twitchGrid.SelectedItem;
                    if (MessageBox.Show(item.Owner + "を削除しますか？", string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        return;

                    if (Twitch.EnableChange)

                        //選択されている項目を削除
                        Twitch.List.Remove(item);
                    MessageBox.Show(item.Owner + "を削除しました");
                    App.SaveList("Twitch");
                    UpdateDispList();
                }
            }

            //選択中のタブがcavetubeなら
            if (GetSelectedTab() == "cavetubeGrid")
            {
                //選択されている項目があるか
                if (cavetubeGrid.SelectedIndex != -1)
                {
                    StreamClass item = (StreamClass)cavetubeGrid.SelectedItem;
                    if (MessageBox.Show(item.Owner + "を削除しますか？", string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        return;

                    if (Cavetube.EnableChange)

                        //選択されている項目を削除
                        Cavetube.List.Remove(item);
                    MessageBox.Show(item.Owner + "を削除しました");
                    App.SaveList("Cavetube");
                    UpdateDispList();
                }
            }

            //選択中のタブが履歴なら
            if (GetSelectedTab() == "logGrid")
            {
                //選択されている項目があるか
                if (logGrid.SelectedIndex != -1)
                {
                    StreamClass item = (StreamClass)logGrid.SelectedItem;
                    if (MessageBox.Show(item.Owner + "を削除しますか？", string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        return;


                    //選択されている項目を削除
                    StaticClass.logList.Remove(item);
                    MessageBox.Show(item.Owner + "を削除しました");
                    App.SaveList("Log");
                    UpdateDispList();
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
        /// タブ切り替え時の処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDispList();
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
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (kukuluGrid2.SelectedIndex >= 0)
                {
                    var item = (StreamClass)kukuluGrid2.SelectedItem;
                    //同じ名前が登録されていないか
                    if (!Kukulu.List.Exists(x => x.Owner == item.Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Kukulu.List.Add(item);
                        MessageBox.Show(item.Owner + "を追加しました。");
                        App.SaveList("Kukulu");
                        UpdateDispList();
                    }
                }
        }
        private void fc2Grid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (fc2Grid2.SelectedIndex >= 0)
                {
                    var item = (StreamClass)fc2Grid2.SelectedItem;
                    //同じ名前が登録されていないか
                    if (!Fc2.List.Exists(x => x.Owner == item.Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Fc2.List.Add(item);
                        MessageBox.Show(item.Owner + "を追加しました。");
                        App.SaveList("Fc2");
                        UpdateDispList();
                    }
                }
        }
        private void twitchGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (twitchGrid2.SelectedIndex >= 0)
                {
                    var item = (StreamClass)twitchGrid2.SelectedItem;
                    //同じ名前が登録されていないか
                    if (!Twitch.List.Exists(x => x.Owner == item.Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Twitch.List.Add(item);
                        MessageBox.Show(item.Owner + "を追加しました。");
                        App.SaveList("Twitch");
                        UpdateDispList();
                    }
                }
        }
        private void cavetubeGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (cavetubeGrid2.SelectedIndex >= 0)
                {
                    var item = (StreamClass)cavetubeGrid2.SelectedItem;
                    //同じ名前が登録されていないか
                    if (!Cavetube.List.Exists(x => x.Owner == item.Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Cavetube.List.Add(item);
                        MessageBox.Show(item.Owner + "を追加しました。");
                        App.SaveList("Cavetube");
                        UpdateDispList();
                    }
                }
        }

        /// <summary>
        /// タブの内容がダブルクリックされたとき選択URLを開く
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void kukuluGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (kukuluGrid.SelectedIndex >= 0)
                {
                    var url = ((StreamClass)kukuluGrid.SelectedItem).Url;
                    if (url != string.Empty)
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(url);
                }
        }
        private void fc2Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Fc2.EnableChange)
                //選択されている項目があるか
                if (fc2Grid.SelectedIndex >= 0)
                {
                    var url = ((StreamClass)fc2Grid.SelectedItem).Url;
                    if (url != string.Empty)
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(url);
                }

        }
        private void twitchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Twitch.EnableChange)
                //選択されている項目があるか
                if (twitchGrid.SelectedIndex >= 0)
                {
                    var url = ((StreamClass)twitchGrid.SelectedItem).Url;
                    if (url != string.Empty)
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(url);
                }
        }
        private void cavetubeGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Cavetube.EnableChange)
                //選択されている項目があるか
                if (cavetubeGrid.SelectedIndex >= 0)
                {
                    var url = ((StreamClass)cavetubeGrid.SelectedItem).Url;
                    if (url != string.Empty)
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(url);
                }
        }
        private void logGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (logGrid.SelectedIndex >= 0)
            {
                var url = ((StreamClass)logGrid.SelectedItem).Url;
                if (url != string.Empty)
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(url);
            }
        }

        //音量調整機能をつけるまでの応急処置
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var player = new System.Media.SoundPlayer(Path.GetFullPath(@".\resource\favorite.wav"));
            player.Play();
        }
    }
}
