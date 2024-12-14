using System.Windows;
using System.Windows.Controls;

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
    ///     <MyNamespace:EnumCombox/>
    ///
    /// </summary>
    public class EnumComboBox : ComboBox
    {
        //static EnumComboBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));

        public string EnumName
        {
            get { return (string)GetValue(EnumNameProperty); }
            set { SetValue(EnumNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnumName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumNameProperty =
            DependencyProperty.Register(nameof(EnumName), typeof(string), typeof(EnumComboBox), new PropertyMetadata((d, e) =>
            {
                var enumComboBox = (EnumComboBox)d;
                enumComboBox.Items.Clear();
                if (!string.IsNullOrWhiteSpace(enumComboBox.EnumName))
                {
                    var source = Enums.EnumHelper.GetEnum(enumComboBox.EnumName);
                    if (!enumComboBox.IsRequired)
                        source.Insert(0, new Enums.EnumItem());
                    enumComboBox.ItemsSource = source;
                }
                //enumComboBox.SelectedValuePath;
                //enumComboBox.DisplayMemberPath;
            }));

        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRequired.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(nameof(IsRequired), typeof(bool), typeof(EnumComboBox), new PropertyMetadata(false));
    }
}
