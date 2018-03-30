using System;
using System.Collections.ObjectModel;
using System.Windows;
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
        /// 開いているGridの更新
        /// 参考URL
        /// http://oita.oika.me/2014/11/03/wpf-datagrid-binding/
        /// </summary>
        public void UpdateDispList()
        {
            if (Kukulu.EnableChange)
            {
                if (TabControl1.SelectedIndex == 0)
                    kukuluGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(Kukulu.List);
                if (TabControl1.SelectedIndex == 1)
                    kukuluGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Kukulu.All);
            }
            if (Fc2.EnableChange)
            {
                if (TabControl1.SelectedIndex == 2)
                    fc2Grid.ItemsSource = new ReadOnlyCollection<StreamClass>(Fc2.List);
                if (TabControl1.SelectedIndex == 3)
                    fc2Grid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Fc2.All);
            }
            if (Twitch.EnableChange)
            {
                if (TabControl1.SelectedIndex == 4)
                    twitchGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(Twitch.List);
                if (TabControl1.SelectedIndex == 5)
                    twitchGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(Twitch.All);
            }
            if (TabControl1.SelectedIndex == 6)
                logGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.logList);
        }

        /// <summary>
        /// 追加ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Button_add_Click(object sender, RoutedEventArgs e)
        {
            //選択中のタブがkukuluなら
            if (TabControl1.SelectedIndex == 0)
                if (Kukulu.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Kukulu.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Kukulu.List.Add(new StreamClass(Textbox1.Text));
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }

            //選択中のタブがFC2なら
            if (TabControl1.SelectedIndex == 2)
                if (Fc2.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Fc2.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Fc2.List.Add(new StreamClass(Textbox1.Text));
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }

            //選択中のタブがtwitchなら
            if (TabControl1.SelectedIndex == 4)
                if (Twitch.EnableChange)
                    //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                    if (!Twitch.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        Twitch.List.Add(new StreamClass(Textbox1.Text));
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }
        }

        /// <summary>
        /// 削除ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Button_del_Click(object sender, RoutedEventArgs e)
        {
            //選択中のタブがkukuluなら
            if (TabControl1.SelectedIndex == 0)
                if (Kukulu.EnableChange)
                    //選択されている項目があるか
                    if (kukuluGrid.SelectedIndex != -1)
                    {
                        //選択されている項目を削除
                        Kukulu.List.RemoveAt(kukuluGrid.SelectedIndex);
                        UpdateDispList();
                    }

            //選択中のタブがFC2なら
            if (TabControl1.SelectedIndex == 2)
                if (Fc2.EnableChange)
                    //選択されている項目があるか
                    if (fc2Grid.SelectedIndex != -1)
                    {
                        //選択されている項目を削除
                        Fc2.List.RemoveAt(fc2Grid.SelectedIndex);
                        UpdateDispList();
                    }

            //選択中のタブがtwtichなら
            if (TabControl1.SelectedIndex == 4)
                if (Twitch.EnableChange)
                    //選択されている項目があるか
                    if (twitchGrid.SelectedIndex != -1)
                    {
                        //選択されている項目を削除
                        Twitch.List.RemoveAt(twitchGrid.SelectedIndex);
                        UpdateDispList();
                    }

            //選択中のタブが履歴なら
            if (TabControl1.SelectedIndex == 6)
                //選択されている項目があるか
                if (logGrid.SelectedIndex != -1)
                {
                    try
                    {
                        //選択されている項目を削除
                        StaticClass.logList.RemoveAt(logGrid.SelectedIndex);
                        UpdateDispList();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
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
            //押されたキーがエンターなら
            if (e.Key == Key.Return)
            {
                //選択中のタブがkukuluなら
                if (TabControl1.SelectedIndex == 0)
                    if (Kukulu.EnableChange)
                        //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                        if (!Kukulu.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                        {
                            //テキストボックスの内容をお気に入り配信者に追加
                            Kukulu.List.Add(new StreamClass(Textbox1.Text));
                            //内容を削除
                            Textbox1.Text = "";
                            UpdateDispList();
                        }

                //選択中のタブがFC2なら
                if (TabControl1.SelectedIndex == 2)
                    if (Fc2.EnableChange)
                        //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                        if (!Fc2.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                        {
                            //テキストボックスの内容をお気に入り配信者に追加
                            Fc2.List.Add(new StreamClass(Textbox1.Text));
                            //内容を削除
                            Textbox1.Text = "";
                            UpdateDispList();
                        }

                //選択中のタブがtwitchなら
                if (TabControl1.SelectedIndex == 4)
                    if (Twitch.EnableChange)
                        //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                        if (!Twitch.List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                        {
                            //テキストボックスの内容をお気に入り配信者に追加
                            Twitch.List.Add(new StreamClass(Textbox1.Text));
                            //内容を削除
                            Textbox1.Text = "";
                            UpdateDispList();
                        }
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
                        TabControl1.SelectedIndex = 0;
                        kukuluGrid.SelectedIndex = Kukulu.List.Count - 1;
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
                        TabControl1.SelectedIndex = 2;
                        fc2Grid.SelectedIndex = Fc2.List.Count - 1;
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
                        TabControl1.SelectedIndex = 4;
                        twitchGrid.SelectedIndex = Twitch.List.Count - 1;
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
                        System.Diagnostics.Process.Start(Kukulu.List[kukuluGrid.SelectedIndex].Url);
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
                        System.Diagnostics.Process.Start(Fc2.List[fc2Grid.SelectedIndex].Url);
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
                        System.Diagnostics.Process.Start(Twitch.List[twitchGrid.SelectedIndex].Url);
                    }
        }
        private void logGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (logGrid.SelectedIndex != -1)
                if (StaticClass.logList[logGrid.SelectedIndex].Url != "")
                {
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(StaticClass.logList[logGrid.SelectedIndex].Url);
                }
        }
    }
}
