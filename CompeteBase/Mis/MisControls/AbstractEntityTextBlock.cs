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
    ///     <MyNamespace:AbstractEntityTextBlock/>
    ///
    /// </summary>
    public abstract class AbstractEntityTextBlock : TextBlock
    {
        protected MethodInfo? formatMethod;

        /// <summary>
        /// 初始化 AbstractEntityTextBlock 的静态成员。
        /// </summary>
        //static AbstractEntityTextBlock()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractEntityTextBlock), new FrameworkPropertyMetadata(typeof(AbstractEntityTextBlock)));
        //}

        public string ServiceParameter
        {
            get { return (string)GetValue(ServiceParameterProperty); }
            set { SetValue(ServiceParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServiceParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServiceParameterProperty =
            DependencyProperty.Register(nameof(ServiceParameter), typeof(string), typeof(AbstractEntityTextBlock));

        public string DisplayPath
        {
            get { return (string)GetValue(DisplayPathProperty); }
            set { SetValue(DisplayPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPathProperty =
            DependencyProperty.Register(nameof(DisplayPath), typeof(string), typeof(AbstractEntityTextBlock));

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register(nameof(ValuePath), typeof(string), typeof(AbstractEntityTextBlock));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected abstract string? GetDisplay(DataTable entities);

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(AbstractEntityTextBlock), new PropertyMetadata((d, e) =>
            {
                var abstractEntityTextBlock = d as AbstractEntityTextBlock;

                // 取得需要从UI线程向新线程传递的数据。
                //var serviceParameter = abstractEntityTextBlock.ServiceParameter;  // 服务参数。
                //var eentityTextBlockValue = abstractEntityTextBlock.Value;               // 控件的值。
                if (string.IsNullOrWhiteSpace(abstractEntityTextBlock?.ServiceParameter))
                    return;

                //var entity = Threading.ThreadingHelper.Invoke(() => GlobalCommon.EntityDataProvider.GetEntity(abstractEntityTextBlock.ServiceParameter, abstractEntityTextBlock.Value), "Query");
                if (abstractEntityTextBlock.Value is null)
                {
                    abstractEntityTextBlock.Text = string.Empty;
                    return;
                }

                if (long.TryParse(abstractEntityTextBlock.Value.ToString(), out long val))
                {
                    if (0L == val)
                    {
                        abstractEntityTextBlock.Text = string.Empty;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(abstractEntityTextBlock.Format))
                    {
                        if (abstractEntityTextBlock.ServiceParameter == "Operator" && GlobalCommon.CurrentUser!.Id == val)
                        {
                            if (abstractEntityTextBlock.DisplayPath.EndsWith("_Code"))
                            {
                                abstractEntityTextBlock.Text = GlobalCommon.CurrentUser.Code;
                                return;
                            }

                            if (abstractEntityTextBlock.DisplayPath.EndsWith("_Name"))
                            {
                                abstractEntityTextBlock.Text = GlobalCommon.CurrentUser.Name;
                                return;
                            }
                        }
                        else if (abstractEntityTextBlock.ServiceParameter == "Tenant" && GlobalCommon.CurrentTenant!.Id == val)
                        {
                            if (abstractEntityTextBlock.DisplayPath.EndsWith("_Code"))
                            {
                                abstractEntityTextBlock.Text = GlobalCommon.CurrentTenant.Code;
                                return;
                            }

                            if (abstractEntityTextBlock.DisplayPath.EndsWith("_Name"))
                            {
                                abstractEntityTextBlock.Text = GlobalCommon.CurrentTenant.Name;
                                return;
                            }
                        }
                    }
                }

                try
                {
                    var entities = GlobalCommon.GetEntity(abstractEntityTextBlock.ServiceParameter, abstractEntityTextBlock.Value);
                    if (entities is null || 0 == entities.Rows.Count || !entities.Columns.Contains(abstractEntityTextBlock.DisplayPath))
                        return;

                    abstractEntityTextBlock.Text = abstractEntityTextBlock.GetDisplay(entities);
                    //abstractEntityTextBlock.Text = string.IsNullOrWhiteSpace(abstractEntityTextBlock.Format) || abstractEntityTextBlock.formatMethod is null
                    //    ? entity.Rows[0][abstractEntityTextBlock.DisplayPath].ToString()
                    //    : abstractEntityTextBlock.formatMethod.Invoke(null, [entity.Rows[0]])?.ToString();
                }
                catch (InvalidOperationException) { }
            }));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(AbstractEntityTextBlock), new PropertyMetadata((d, e) =>
            {
                var abstractEntityTextBlock = d as AbstractEntityTextBlock;
                if (!string.IsNullOrWhiteSpace(abstractEntityTextBlock?.Format))
                    abstractEntityTextBlock.formatMethod = Scripts.ScriptBuilder.GetMethod(Scripts.ScriptTemplates.FormatTemplate, abstractEntityTextBlock.Format, "Compete.Scripts.Formater", "GetString");
            }));

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
                { ValuePathProperty, parameters is null || ! parameters.TryGetValue(nameof(ValuePath), out string ? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters is null || ! parameters.TryGetValue(nameof(DisplayPath), out string ? displayPath) ? $"{entityName}_Name" : displayPath },
                { ServiceParameterProperty, parameters is null || !parameters.TryGetValue(nameof(ServiceParameter), out string? serviceParameter) ? entityName : serviceParameter }
            };
        }

        public static IDictionary<DependencyProperty, object?> GenerateGridPropertyDictionary(DataColumn column, IDictionary<string, string>? parameters)
        {
            var entityName = GetEntityName(column.ColumnName);

            return new Dictionary<DependencyProperty, object?>
            {
                { ValuePathProperty, parameters is null || ! parameters.TryGetValue(nameof(ValuePath), out string ? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters is null || ! parameters.TryGetValue(nameof(DisplayPath), out string ? displayPath) ? $"{entityName}_Name" : displayPath },
                { ServiceParameterProperty, parameters is null || !parameters.TryGetValue(nameof(ServiceParameter), out string? serviceParameter) ? entityName : serviceParameter }
            };
        }
    }
}
