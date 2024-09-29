using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Compete.Mis.MisControls
{
    public class DataTreeView : TreeView
    {
        //static DataTreeView()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeView), new FrameworkPropertyMetadata(typeof(TreeView)));
        //}

        public DataTreeView() => DataContextChanged += (sender, e) => Refresh();

        private void Refresh()
        {
            Items.Clear();

            if (string.IsNullOrWhiteSpace(ParentPath) || string.IsNullOrWhiteSpace(ChildPath))
                return;

            var source = DataSource ?? DataContext;
            if (source is null)
                return;

            if (source is not DataView view)
                if (source is DataTable table)
                    view = table.DefaultView;
                else if (source is BindingListCollectionView collectionView && collectionView.SourceCollection is DataView sourceCollection)
                    view = sourceCollection;
                else
                    return;

            var list = new List<DataRowView>();
            foreach (DataRowView item in view)
                list.Add(item);

            isBinding = true;
            AddItems(Items, list, RootValue);
            isBinding = false;
        }

        private bool isBinding = false;

        private void AddItems(ItemCollection items, IEnumerable<DataRowView> data, object itemValue)
        {
            var rows = (from row in data
                        where row[ParentPath].Equals(itemValue)
                        select row).OrderBy(row => row[string.IsNullOrWhiteSpace(OrderPath) ? ChildPath : OrderPath]);

            TreeViewItem item;
            CheckBox checkBox;
            string displayPath = string.IsNullOrWhiteSpace(DisplayPath) ? ChildPath : DisplayPath;
            foreach (var row in rows)
            {
                item = new TreeViewItem();
                if (string.IsNullOrWhiteSpace(CheckPath))
                    item.Header = row[displayPath];
                else
                {
                    checkBox = new CheckBox
                    {
                        Tag = row,
                    };
                    checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(CheckPath) { Source = row, });
                    checkBox.SetBinding(ContentControl.ContentProperty, new Binding(displayPath) { Source = row, });
                    checkBox.Checked += (sender, e) =>
                    {
                        if (isBinding)
                            return;

                        var parent = ((FrameworkElement)((FrameworkElement)sender).Parent).Parent;
                        if (parent is HeaderedItemsControl headeredItemsControl)
                            ((ToggleButton)headeredItemsControl.Header).IsChecked = true;
                    };
                    checkBox.Unchecked += (sender, e) =>
                    {
                        if (isBinding)
                            return;

                        var parent = (ItemsControl)((FrameworkElement)sender).Parent;
                        foreach(var item in parent.Items)
                            ((ToggleButton)((HeaderedItemsControl)item).Header).IsChecked = false;
                    };
                    item.Header = checkBox;
                }
                if (!string.IsNullOrWhiteSpace(ToolTipPath))
                    item.SetBinding(ToolTipProperty, new Binding(ToolTipPath) { Source = row, });

                AddItems(item.Items, data, row[ChildPath]);
                items.Add(item);
            }
        }

        public string ParentPath
        {
            get { return (string)GetValue(ParentPathProperty); }
            set { SetValue(ParentPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPathProperty =
            DependencyProperty.Register(nameof(ParentPath), typeof(string), typeof(DataTreeView), new PropertyMetadata((d, e) => ((DataTreeView)d).Refresh()));

        public string ChildPath
        {
            get { return (string)GetValue(ChildPathProperty); }
            set { SetValue(ChildPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildPathProperty =
            DependencyProperty.Register(nameof(ChildPath), typeof(string), typeof(DataTreeView), new PropertyMetadata((d, e) => ((DataTreeView)d).Refresh()));

        public string OrderPath
        {
            get { return (string)GetValue(OrderPathProperty); }
            set { SetValue(OrderPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderPathProperty =
            DependencyProperty.Register(nameof(OrderPath), typeof(string), typeof(DataTreeView), new PropertyMetadata((d, e) => ((DataTreeView)d).Refresh()));

        public string CheckPath
        {
            get { return (string)GetValue(CheckPathProperty); }
            set { SetValue(CheckPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckPathProperty =
            DependencyProperty.Register(nameof(CheckPath), typeof(string), typeof(DataTreeView), new PropertyMetadata((d, e) => ((DataTreeView)d).Refresh()));

        public string DisplayPath
        {
            get { return (string)GetValue(DisplayPathProperty); }
            set { SetValue(DisplayPathProperty, value); }
        }

        public string ToolTipPath
        {
            get { return (string)GetValue(ToolTipPathProperty); }
            set { SetValue(ToolTipPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTipPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipPathProperty =
            DependencyProperty.Register(nameof(ToolTipPath), typeof(string), typeof(DataTreeView), new PropertyMetadata((d, e) => ((DataTreeView)d).Refresh()));

        // Using a DependencyProperty as the backing store for DisplayPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPathProperty =
            DependencyProperty.Register(nameof(DisplayPath), typeof(string), typeof(DataTreeView), new PropertyMetadata((d, e) => ((DataTreeView)d).Refresh()));

        //public bool IsReadOnly
        //{
        //    get { return (bool)GetValue(IsReadOnlyProperty); }
        //    set { SetValue(IsReadOnlyProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsReadOnlyProperty =
        //    DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataTreeView)));

        public object RootValue
        {
            get { return (object)GetValue(RootValueProperty); }
            set { SetValue(RootValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RootValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RootValueProperty =
            DependencyProperty.Register(nameof(RootValue), typeof(object), typeof(DataTreeView), new PropertyMetadata(0L));

        public IEnumerable DataSource
        {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(nameof(DataSource), typeof(IEnumerable), typeof(DataTreeView), new PropertyMetadata((d, e) =>
            {
                ((DataTreeView)d).Refresh();
            }));
    }
}
