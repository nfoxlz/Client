// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/1/26 周六 12:20:12 LeeZheng 新建。
// ======================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
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
    ///     <MyNamespace:EntityTextBlock/>
    ///
    /// </summary>
    public class EntityTextBlock : TextBlock
    {
        private MethodInfo? formatMethod;

        /// <summary>
        /// 初始化 EntityTextBlock 的静态成员。
        /// </summary>
        //static EntityTextBlock()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityTextBlock), new FrameworkPropertyMetadata(typeof(EntityTextBlock)));
        //}

        public string ServiceParameter
        {
            get { return (string)GetValue(ServiceParameterProperty); }
            set { SetValue(ServiceParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServiceParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServiceParameterProperty =
            DependencyProperty.Register(nameof(ServiceParameter), typeof(string), typeof(EntityTextBlock));

        public string DisplayPath
        {
            get { return (string)GetValue(DisplayPathProperty); }
            set { SetValue(DisplayPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPathProperty =
            DependencyProperty.Register(nameof(DisplayPath), typeof(string), typeof(EntityTextBlock));

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register(nameof(ValuePath), typeof(string), typeof(EntityTextBlock));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(EntityTextBlock), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Value 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityTextBlock = d as EntityTextBlock;

            // 取得需要从UI线程向新线程传递的数据。
            //var serviceParameter = entityTextBlock.ServiceParameter;  // 服务参数。
            //var eentityTextBlockValue = entityTextBlock.Value;               // 控件的值。
            if (string.IsNullOrWhiteSpace(entityTextBlock?.ServiceParameter))
                return;

            //var entity = Threading.ThreadingHelper.Invoke(() => GlobalCommon.EntityDataProvider.GetEntity(entityTextBlock.ServiceParameter, entityTextBlock.Value), "Query");
            if (entityTextBlock.Value == null)
            {
                entityTextBlock.Text = string.Empty;
                return;
            }

            if (long.TryParse(entityTextBlock.Value.ToString(), out long val))
            {
                if (val == 0L)
                {
                    entityTextBlock.Text = string.Empty;
                    return;
                }

                if (string.IsNullOrWhiteSpace(entityTextBlock.Format))
                {
                    if (entityTextBlock.ServiceParameter == "Operator" && val == GlobalCommon.CurrentUser!.Id)
                    {
                        if (entityTextBlock.DisplayPath.EndsWith("_Code"))
                        {
                            entityTextBlock.Text = GlobalCommon.CurrentUser.Code;
                            return;
                        }

                        if (entityTextBlock.DisplayPath.EndsWith("_Name"))
                        {
                            entityTextBlock.Text = GlobalCommon.CurrentUser.Name;
                            return;
                        }
                    }
                    else if (entityTextBlock.ServiceParameter == "Tenant" && val == GlobalCommon.CurrentTenant!.Id)
                    {
                        if (entityTextBlock.DisplayPath.EndsWith("_Code"))
                        {
                            entityTextBlock.Text = GlobalCommon.CurrentTenant.Code;
                            return;
                        }

                        if (entityTextBlock.DisplayPath.EndsWith("_Name"))
                        {
                            entityTextBlock.Text = GlobalCommon.CurrentTenant.Name;
                            return;
                        }
                    }
                }
            }

            try
            {
                var entity = GlobalCommon.EntityDataProvider!.GetEntity(entityTextBlock.ServiceParameter, entityTextBlock.Value);
                if (entity == null || entity.Rows.Count == 0 || !entity.Columns.Contains(entityTextBlock.DisplayPath))
                    return;

                if (string.IsNullOrWhiteSpace(entityTextBlock.Format) || entityTextBlock.formatMethod == null)
                    entityTextBlock.Text = entity.Rows[0][entityTextBlock.DisplayPath].ToString();
                else
                    entityTextBlock.Text = entityTextBlock.formatMethod.Invoke(null, new object[] { entity.Rows[0] })?.ToString();
            }
            catch(InvalidOperationException) { }
        }

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(EntityTextBlock), new PropertyMetadata(new PropertyChangedCallback(OnFormatChanged)));

        /// <summary>
        /// Format 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityTextBlock = d as EntityTextBlock;
            if (!string.IsNullOrWhiteSpace(entityTextBlock?.Format))
                entityTextBlock.formatMethod = Scripts.ScriptBuilder.GetMethod(Scripts.ScriptTemplates.FormatTemplate, entityTextBlock.Format, "Compete.Scripts.Formater", "GetString");
        }

        private static string GetEntityName(string columnName)
        {
            var entityName = columnName.EndsWith("_Id") ? columnName[..^3] : columnName;
            if (entityName.EndsWith("_User"))
                entityName = "Operator";
            return entityName;
        }

        public static IDictionary<DependencyProperty, object?> GeneratePropertyDictionary(DataColumn column, IDictionary<string, string>? parameters)
        {
            var entityName = GetEntityName(column.ColumnName);

            return new Dictionary<DependencyProperty, object?>
            {
                { ValuePathProperty, parameters == null || ! parameters.TryGetValue("ValuePath", out string ? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters == null || ! parameters.TryGetValue("DisplayPath", out string ? displayPath) ? $"{entityName}_Name" : displayPath },
                { ServiceParameterProperty, parameters == null || !parameters.TryGetValue("ServiceParameter", out string? serviceParameter) ? entityName : serviceParameter }
            };
        }

        public static IDictionary<DependencyProperty, object?> GenerateGridPropertyDictionary(DataColumn column, IDictionary<string, string>? parameters)
        {
            var entityName = GetEntityName(column.ColumnName);

            return new Dictionary<DependencyProperty, object?>
            {
                { ValuePathProperty, parameters == null || ! parameters.TryGetValue("ValuePath", out string ? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters == null || ! parameters.TryGetValue("DisplayPath", out string ? displayPath) ? $"{entityName}_Code" : displayPath },
                { ServiceParameterProperty, parameters == null || !parameters.TryGetValue("ServiceParameter", out string? serviceParameter) ? entityName : serviceParameter }
            };
        }
    }
}
