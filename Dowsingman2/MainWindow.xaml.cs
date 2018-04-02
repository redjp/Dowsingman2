using System;
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
                    kukuluGrid.Focus();
                }
                if (GetSelectedTab() == "kukuluGrid2")
                {
                    kukuluGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Kukulu.All);
                    SortList(kukuluGrid2);
                    kukuluGrid2.Focus();
                }
            }
            if (Fc2.EnableChange)
            {
                if (GetSelectedTab() == "fc2Grid")
                {
                    fc2Grid.ItemsSource = new ReadOnlyCollection<StreamClass>(Fc2.List);
                    SortList(fc2Grid);
                    fc2Grid.Focus();
                }
                if (GetSelectedTab() == "fc2Grid2")
                {
                    fc2Grid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Fc2.All);
                    SortList(fc2Grid2);
                    fc2Grid2.Focus();
                }
            }
            if (Twitch.EnableChange)
            {
                if (GetSelectedTab() == "twitchGrid")
                {
                    twitchGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(Twitch.List);
                    SortList(twitchGrid);
                    twitchGrid.Focus();
                }
                if (GetSelectedTab() == "twitchGrid2")
                {
                    twitchGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Twitch.All);
                    SortList(twitchGrid2);
                    twitchGrid2.Focus();
                }
            }
            if (GetSelectedTab() == "logGrid")
            {
                logGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.logList);
                SortList(logGrid);
                logGrid.Focus();
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
                    if (!Kukulu.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Kukulu.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }

            //選択中のタブがFC2なら
            if (GetSelectedTab() == "fc2Grid")
                if (Fc2.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Fc2.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Fc2.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }

            //選択中のタブがtwitchなら
            if (GetSelectedTab() == "twitchGrid")
                if (Twitch.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Twitch.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Twitch.List.Add(new StreamClass(Textbox1.Text));
                        MessageBox.Show(Textbox1.Text + "を追加しました。");
                        //内容を削除
                        Textbox1.Text = "";
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
                if (Kukulu.EnableChange)
                    //選択されている項目があるか
                    if (kukuluGrid.SelectedIndex != -1)
                    {
                        //選択されている項目を削除
                        StreamClass item = (StreamClass)kukuluGrid.SelectedItem;
                        Kukulu.List.Remove(item);
                        MessageBox.Show(item.Owner + "を削除しました");
                        UpdateDispList();
                    }

            //選択中のタブがFC2なら
            if (GetSelectedTab() == "fc2Grid")
                if (Fc2.EnableChange)
                    //選択されている項目があるか
                    if (fc2Grid.SelectedIndex != -1)
                    {
                        //選択されている項目を削除
                        StreamClass item = (StreamClass)fc2Grid.SelectedItem;
                        Fc2.List.Remove(item);
                        MessageBox.Show(item.Owner + "を削除しました");
                        UpdateDispList();
                    }

            //選択中のタブがtwtichなら
            if (GetSelectedTab() == "twitchGrid")
                if (Twitch.EnableChange)
                    //選択されている項目があるか
                    if (twitchGrid.SelectedIndex != -1)
                    {
                        //選択されている項目を削除
                        StreamClass item = (StreamClass)twitchGrid.SelectedItem;
                        Twitch.List.Remove(item);
                        MessageBox.Show(item.Owner + "を削除しました");
                        UpdateDispList();
                    }

            //選択中のタブが履歴なら
            if (GetSelectedTab() == "logGrid")
                //選択されている項目があるか
                if (logGrid.SelectedIndex != -1)
                {
                    //選択されている項目を削除
                    StreamClass item = (StreamClass)logGrid.SelectedItem;
                    StaticClass.logList.Remove(item);
                    MessageBox.Show(item.Owner + "を削除しました");
                    UpdateDispList();
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
                if (kukuluGrid2.SelectedIndex != -1)
                    //同じ名前が登録されていないか
                    if (!Kukulu.List.Exists(item => item.Owner == Kukulu.All[kukuluGrid2.SelectedIndex].Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Kukulu.List.Add(Kukulu.All[kukuluGrid2.SelectedIndex]);
                        MessageBox.Show(Kukulu.All[kukuluGrid2.SelectedIndex].Owner + "を追加しました。");
                        UpdateDispList();
                    }
        }
        private void fc2Grid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (fc2Grid2.SelectedIndex != -1)
                    //同じ名前が登録されていないか
                    if (!Fc2.List.Exists(item => item.Owner == Fc2.All[fc2Grid2.SelectedIndex].Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Fc2.List.Add(Fc2.All[fc2Grid2.SelectedIndex]);
                        MessageBox.Show(Fc2.All[fc2Grid2.SelectedIndex].Owner + "を追加しました。");
                        UpdateDispList();
                    }
        }
        private void twitchGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Kukulu.EnableChange)
                //選択されている項目があるか
                if (twitchGrid2.SelectedIndex != -1)
                    //同じ名前が登録されていないか
                    if (!Twitch.List.Exists(item => item.Owner == Twitch.All[twitchGrid2.SelectedIndex].Owner))
                    {
                        //選択中の項目をお気に入り配信者に追加
                        Twitch.List.Add(Twitch.All[twitchGrid2.SelectedIndex]);
                        MessageBox.Show(Twitch.All[twitchGrid2.SelectedIndex].Owner + "を追加しました。");
                        UpdateDispList();
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
                if (kukuluGrid.SelectedIndex != -1)
                    if (Kukulu.List[kukuluGrid.SelectedIndex].Url != "")
                    {
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(((StreamClass)kukuluGrid.SelectedItem).Url);
                    }
        }
        private void fc2Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Fc2.EnableChange)
                //選択されている項目があるか
                if (fc2Grid.SelectedIndex != -1)
                    if (Fc2.List[fc2Grid.SelectedIndex].Url != "")
                    {
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(((StreamClass)fc2Grid.SelectedItem).Url);
                    }

        }
        private void twitchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Twitch.EnableChange)
                //選択されている項目があるか
                if (twitchGrid.SelectedIndex != -1)
                    if (Twitch.List[twitchGrid.SelectedIndex].Url != "")
                    {
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(((StreamClass)twitchGrid.SelectedItem).Url);
                    }
        }
        private void logGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (logGrid.SelectedIndex != -1)
                if (StaticClass.logList[logGrid.SelectedIndex].Url != "")
                {
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(((StreamClass)logGrid.SelectedItem).Url);
                }
        }
    }
}
