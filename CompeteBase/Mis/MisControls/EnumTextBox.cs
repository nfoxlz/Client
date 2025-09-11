using System.Collections.Generic;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.MisControls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.MisControls;assembly=Compete.Mis.MisControls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:EnumTextBox/>
    ///
    /// </summary>
    public class EnumTextBox : WatermarkTextBox
    {
        static EnumTextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));

        private readonly Dictionary<sbyte, string?> enumDictionary = [];

        public EnumTextBox()
        {
            IsReadOnly = true;
            Height = GlobalConstants.TextBoxHeight;
        }

        private void RefreshValue()
        {
            if (enumDictionary.TryGetValue(Value, out string? text))
                Text = text;
        }

        public string EnumName
        {
            get { return (string)GetValue(EnumNameProperty); }
            set { SetValue(EnumNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnumName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumNameProperty =
            DependencyProperty.Register(nameof(EnumName), typeof(string), typeof(EnumTextBox), new PropertyMetadata((d, e) =>
            {
                var enumTextBox = (EnumTextBox)d;
                enumTextBox.enumDictionary.Clear();
                if (!string.IsNullOrWhiteSpace(enumTextBox.EnumName))
                    foreach (var item in Enums.EnumHelper.GetEnum(enumTextBox.EnumName))
                        if (null != item.Value)
                            enumTextBox.enumDictionary.Add(item.Value.Value, item.DisplayName);
                enumTextBox.RefreshValue();
            }));

        public sbyte Value
        {
            get { return (sbyte)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(sbyte), typeof(EnumTextBox), new PropertyMetadata((d, e) => ((EnumTextBox)d).RefreshValue()));
    }
}
