using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

/// <summary>
/// 参考URL
/// http://running-cs.hatenablog.com/entry/2016/08/27/113755
/// </summary>
namespace DataGridSort
{
    public class Attached
    {
        public static bool GetIsSortCustomize(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSortCustomizeProperty);
        }

        public static void SetIsSortCustomize(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSortCustomizeProperty, value);
        }

        //Trueに設定した場合、ソートを昇順、降順、なしの順で行われるカスタムが施される
        public static readonly DependencyProperty IsSortCustomizeProperty =
            DependencyProperty.RegisterAttached("IsSortCustomize", typeof(bool), typeof(Attached), new PropertyMetadata(OnIsSortCustomizeChanged));

        private static void OnIsSortCustomizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datagrid = d as DataGrid;
            if (datagrid == null) return;
            if ((bool)e.NewValue)
            {
                datagrid.Sorting += Datagrid_Sorting;
            }
            else
            {
                datagrid.Sorting -= Datagrid_Sorting;
            }
        }

        private static void Datagrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var datagrid = sender as DataGrid;
            if (datagrid.ItemsSource == null) return;

            var listColView = (ListCollectionView)CollectionViewSource.GetDefaultView(datagrid.ItemsSource);
            if (listColView == null) return;

            if (e.Column.SortDirection == ListSortDirection.Descending)
            {
                //ソートを中断
                e.Handled = true;
                //ソートの方向をクリア
                e.Column.SortDirection = null;
                datagrid.Items.SortDescriptions.Clear();
            }
        }
    }
}