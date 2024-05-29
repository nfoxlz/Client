// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/14 10:52:01 LeeZheng 新建。
//==============================================================
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 选择框控件类。
    /// </summary>
    public abstract class ChoiceBox : WrapPanel
    {
        /// <summary>
        /// 项目字典。
        /// </summary>
        private readonly Dictionary<ToggleButton, sbyte> itemDictionary = [];

        /// <summary>
        /// 静态构造方法。
        /// </summary>
        static ChoiceBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ChoiceBox), new FrameworkPropertyMetadata(typeof(WrapPanel)));

        /// <summary>
        /// 项目单击事件处理方法。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件数据。</param>
        private void Item_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            OnIsCheckedChanged(itemDictionary[button!], button!.IsChecked == true);
        }

        /// <summary>
        /// 创建项目。
        /// </summary>
        private void CreateItems()
        {
            // 清空。
            Children.Clear();
            itemDictionary.Clear();

            if (null == ItemData)
                return;

            // 设置项目。
            ToggleButton item;
            foreach (var pair in ItemData)
            {
                item = CreateItem();
                item.Margin = new Thickness(0D, 3D, 5D, 0D);
                item.Content = pair.Value;
                itemDictionary.Add(item, pair.Key);
                item.Click += Item_Click;
                Children.Add(item);
            }
        }

        /// <summary>
        /// 显示值。
        /// </summary>
        private void ShowValue()
        {
            foreach (var pair in itemDictionary)
                ShowItem(pair.Key, pair.Value);
        }

        /// <summary>
        /// 显示项目。
        /// </summary>
        /// <param name="item">项目控件。</param>
        /// <param name="itemValue">项目值。</param>
        protected abstract void ShowItem(ToggleButton item, sbyte itemValue);

        /// <summary>
        /// 创建单个项目。
        /// </summary>
        /// <returns>单个项目。</returns>
        protected abstract ToggleButton CreateItem();

        /// <summary>
        /// 项目的 IsChecked 属性改变时，调用该方法。
        /// </summary>
        /// <param name="value">项目的值。</param>
        /// <param name="isChecked">IsChecked 属性的值。</param>
        protected abstract void OnIsCheckedChanged(sbyte value, bool isChecked);

        #region 依赖属性

        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        public long Value
        {
            get { return (long)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 标识 Value 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(long), typeof(ChoiceBox), new PropertyMetadata((d, e) => (d as ChoiceBox)?.ShowValue()));

        /// <summary>
        /// 获取或设置一个值，标识控件是否是只读的。
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 标识 IsReadOnly 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ChoiceBox), new PropertyMetadata((d, e) =>
            {
                var choiceBox = d as ChoiceBox;
                foreach (var item in choiceBox!.itemDictionary.Keys)
                    item.IsEnabled = !choiceBox.IsReadOnly;
            }));

        /// <summary>
        /// 获取或设置控件项目的数据。
        /// </summary>
        public IDictionary<sbyte, string> ItemData
        {
            get { return (IDictionary<sbyte, string>)GetValue(ItemDataProperty); }
            set { SetValue(ItemDataProperty, value); }
        }

        /// <summary>
        /// 标识 ItemData 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ItemDataProperty =
            DependencyProperty.Register(nameof(ItemData), typeof(IDictionary<sbyte, string>), typeof(ChoiceBox), new PropertyMetadata((d, e) => (d as ChoiceBox)?.CreateItems()));

        #endregion 依赖属性
    }
}
