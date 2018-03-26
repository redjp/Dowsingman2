using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Gridの更新
        /// 参考URL
        /// http://oita.oika.me/2014/11/03/wpf-datagrid-binding/
        /// </summary>
        public void UpdateDispList()
        {
            kukuluGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.kukuluList);
            kukuluGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.kukuluAll);
            fc2Grid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.fc2List);
            fc2Grid2.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.fc2All);
            twitchGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.twitchList);
            twitchGrid2.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.twitchAll);
            logGrid.ItemsSource = new ReadOnlyCollection<StreamClass>(StaticClass.logList);
        }

        /// <summary>
        /// 追加ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_add_Click(object sender, RoutedEventArgs e)
        {
            //選択中のタブがkukuluなら
            if (TabControl1.SelectedIndex == 0)
            {
                //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                if (!StaticClass.kukuluList.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                {
                    //テキストボックスの内容をお気に入り配信者に追加
                    StaticClass.kukuluList.Add(new StreamClass(Textbox1.Text));
                    //内容を削除
                    Textbox1.Text = "";
                    UpdateDispList();
                }
            }
            //選択中のタブがFC2なら
            if (TabControl1.SelectedIndex == 2)
            {
                //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                if (!StaticClass.fc2List.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                {
                    //テキストボックスの内容をお気に入り配信者に追加
                    StaticClass.fc2List.Add(new StreamClass(Textbox1.Text));
                    //内容を削除
                    Textbox1.Text = "";
                    UpdateDispList();
                }
            }
            //選択中のタブがtwitchなら
            if (TabControl1.SelectedIndex == 4)
            {
                //同じ名前が登録されていないか、テキストボックスが空欄じゃないか
                if (!StaticClass.twitchList.Exists(item => item.Owner == Textbox1.Text) && Textbox1.Text != "")
                {
                    //テキストボックスの内容をお気に入り配信者に追加
                    StaticClass.twitchList.Add(new StreamClass(Textbox1.Text));
                    //内容を削除
                    Textbox1.Text = "";
                    UpdateDispList();
                }
            }
        }

        /// <summary>
        /// 削除ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_del_Click(object sender, RoutedEventArgs e)
        {
            //選択中のタブがkukuluなら
            if (TabControl1.SelectedIndex == 0)
            {
                //選択されている項目があるか
                if (kukuluGrid.SelectedIndex != -1)
                {
                    //選択されている項目を削除
                    StaticClass.kukuluList.RemoveAt(kukuluGrid.SelectedIndex);
                    UpdateDispList();
                }
            }
            //選択中のタブがFC2なら
            if (TabControl1.SelectedIndex == 2)
            {
                //選択されている項目があるか
                if (fc2Grid.SelectedIndex != -1)
                {
                    //選択されている項目を削除
                    StaticClass.fc2List.RemoveAt(fc2Grid.SelectedIndex);
                    UpdateDispList();
                }
            }
            //選択中のタブがtwtichなら
            if (TabControl1.SelectedIndex == 4)
            {
                //選択されている項目があるか
                if (twitchGrid.SelectedIndex != -1)
                {
                    //選択されている項目を削除
                    StaticClass.twitchList.RemoveAt(twitchGrid.SelectedIndex);
                    UpdateDispList();
                }
            }
            //選択中のタブが履歴なら
            if (TabControl1.SelectedIndex == 6)
            {
                //選択されている項目があるか
                if (logGrid.SelectedIndex != -1)
                {
                    //選択されている項目を削除
                    StaticClass.logList.RemoveAt(logGrid.SelectedIndex);
                    UpdateDispList();
                }
            }
        }

        /// <summary>
        /// タブ切り替え時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDispList();
        }

        /// <summary>
        /// テキストボックスでキー入力があった場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Textbox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //押されたキーがエンターなら
            if (e.Key == Key.Return)
            {
                //選択中のタブがkukuluなら
                if (TabControl1.SelectedIndex == 0)
                {
                    //同じ名前が登録されていないか
                    if (!StaticClass.kukuluList.Exists(item => item.Owner == Textbox1.Text))
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        StaticClass.kukuluList.Add(new StreamClass(Textbox1.Text));
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }
                }
                //選択中のタブがFC2なら
                if (TabControl1.SelectedIndex == 2)
                {
                    //同じ名前が登録されていないか
                    if (!StaticClass.fc2List.Exists(item => item.Owner == Textbox1.Text))
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        StaticClass.fc2List.Add(new StreamClass(Textbox1.Text));
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }
                }
                //選択中のタブがtwitchなら
                if (TabControl1.SelectedIndex == 4)
                {
                    //同じ名前が登録されていないか
                    if (!StaticClass.twitchList.Exists(item => item.Owner == Textbox1.Text))
                    {
                        //テキストボックスの内容をお気に入り配信者に追加
                        StaticClass.twitchList.Add(new StreamClass(Textbox1.Text));
                        //内容を削除
                        Textbox1.Text = "";
                        UpdateDispList();
                    }
                }
            }
        }

        /// <summary>
        /// 一覧タブの内容がダブルクリックされたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kukuluGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (kukuluGrid2.SelectedIndex != -1)
            {
                //同じ名前が登録されていないか
                if (!StaticClass.kukuluList.Exists(item => item.Owner == StaticClass.kukuluAll[kukuluGrid2.SelectedIndex].Owner))
                {
                    //選択中の項目をお気に入り配信者に追加
                    StaticClass.kukuluList.Add(StaticClass.kukuluAll[kukuluGrid2.SelectedIndex]);
                    UpdateDispList();
                    TabControl1.SelectedIndex = 0;
                    kukuluGrid.SelectedIndex = StaticClass.kukuluList.Count - 1;
                }
            }
        }

        private void fc2Grid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (fc2Grid2.SelectedIndex != -1)
            {
                //同じ名前が登録されていないか
                if (!StaticClass.fc2List.Exists(item => item.Owner == StaticClass.fc2All[fc2Grid2.SelectedIndex].Owner))
                {
                    //選択中の項目をお気に入り配信者に追加
                    StaticClass.fc2List.Add(StaticClass.fc2All[fc2Grid2.SelectedIndex]);
                    UpdateDispList();
                    TabControl1.SelectedIndex = 2;
                    fc2Grid.SelectedIndex = StaticClass.fc2List.Count - 1;
                }
            }
        }

        private void twitchGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (twitchGrid2.SelectedIndex != -1)
            {
                //同じ名前が登録されていないか
                if (!StaticClass.twitchList.Exists(item => item.Owner == StaticClass.twitchAll[twitchGrid2.SelectedIndex].Owner))
                {
                    //選択中の項目をお気に入り配信者に追加
                    StaticClass.twitchList.Add(StaticClass.twitchAll[twitchGrid2.SelectedIndex]);
                    UpdateDispList();
                    TabControl1.SelectedIndex = 4;
                    twitchGrid.SelectedIndex = StaticClass.twitchList.Count - 1;
                }
            }
        }

        /// <summary>
        /// kukuluタブの内容がダブルクリックされたとき選択URLを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kukuluGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (kukuluGrid.SelectedIndex != -1)
            {
                if (StaticClass.kukuluList[kukuluGrid.SelectedIndex].Url != "")
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(StaticClass.kukuluList[kukuluGrid.SelectedIndex].Url);
            }
        }

        private void fc2Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (fc2Grid.SelectedIndex != -1)
            {
                if (StaticClass.fc2List[fc2Grid.SelectedIndex].Url != "")
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(StaticClass.fc2List[fc2Grid.SelectedIndex].Url);
            }
        }

        private void twitchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (twitchGrid.SelectedIndex != -1)
            {
                if (StaticClass.twitchList[twitchGrid.SelectedIndex].Url != "")
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(StaticClass.twitchList[twitchGrid.SelectedIndex].Url);
            }
        }

        private void logGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //選択されている項目があるか
            if (logGrid.SelectedIndex != -1)
            {
                if (StaticClass.logList[logGrid.SelectedIndex].Url != "")
                    //規定のブラウザで配信URLを開く
                    System.Diagnostics.Process.Start(StaticClass.logList[logGrid.SelectedIndex].Url);
            }
        }
    }
}
