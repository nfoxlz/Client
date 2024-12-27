using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Compete.Mis.MisControls
{
    public sealed partial class EnumTextBlock : TextBlock
    {
        //static EnumTextBlock() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(typeof(TextBlock)));

        private readonly Dictionary<sbyte, string?> enumDictionary = [];

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
            DependencyProperty.Register(nameof(EnumName), typeof(string), typeof(EnumTextBlock), new PropertyMetadata((d, e) =>
            {
                var enumTextBlock = (EnumTextBlock)d;
                enumTextBlock.enumDictionary.Clear();
                if (!string.IsNullOrWhiteSpace(enumTextBlock.EnumName))
                    foreach (var item in Enums.EnumHelper.GetEnum(enumTextBlock.EnumName))
                        if (null != item.Value)
                            enumTextBlock.enumDictionary.Add(item.Value.Value, item.DisplayName);
                enumTextBlock.RefreshValue();
            }));

        public sbyte Value
        {
            get { return (sbyte)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(sbyte), typeof(EnumTextBlock), new PropertyMetadata((d, e) => ((EnumTextBlock)d).RefreshValue()));
    }
}
