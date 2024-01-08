// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/1/24 周四 9:11:58 LeeZheng 新建。
// ======================================================
using System.Windows;
using System.Windows.Controls;
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
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.Controls;assembly=Compete.Mis.Controls"
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
    ///     <MyNamespace:NameBox/>
    ///
    /// </summary>
    public class NameBox : WatermarkTextBox
    {
        /// <summary>
        /// 初始化 NameBox 的静态成员。
        /// </summary>
        //static NameBox()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(NameBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        //}

        public NameBox()
        {
            Height = 23D;
            TextChanged += NameBox_TextChanged;
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e) => MnemonicCode = Utils.Chinese.GetSpell(Text);

        //public string NameData
        //{
        //    get { return (string)GetValue(NameDataProperty); }
        //    set { SetValue(NameDataProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for NameData.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NameDataProperty =
        //    DependencyProperty.Register("NameData", typeof(string), typeof(NameBox), new PropertyMetadata(new PropertyChangedCallback(OnNameDataChanged)));

        ///// <summary>
        ///// NameData 依赖项属性更变的回调方法。
        ///// </summary>
        ///// <param name="d">属性已更改值的 DependencyObject 。</param>
        ///// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        //private static void OnNameDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var nameBox = d as NameBox;
        //    nameBox.MnemonicCode = Utils.Chinese.GetSpell(nameBox.NameData);
        //}

        public string MnemonicCode
        {
            get { return (string)GetValue(MnemonicCodeProperty); }
            set { SetValue(MnemonicCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MnemonicCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MnemonicCodeProperty =
            DependencyProperty.Register("MnemonicCode", typeof(string), typeof(NameBox));
    }
}
