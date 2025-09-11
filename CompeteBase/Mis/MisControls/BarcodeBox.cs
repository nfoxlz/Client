using SkiaSharp.Views.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace Compete.Mis.MisControls
{
    public class BarcodeBox : Image
    {
        public BarcodeBox() => SizeChanged += (s, e) => Refresh(Convert.ToInt32(e.NewSize.Width), Convert.ToInt32(e.NewSize.Height));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(BarcodeBox), new PropertyMetadata((d, e) => ((BarcodeBox)d).Refresh()));

        public BarcodeFormat Format
        {
            get { return (BarcodeFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(BarcodeFormat), typeof(BarcodeBox), new PropertyMetadata(BarcodeFormat.CODE_128, (d, e) => ((BarcodeBox)d).Refresh()));

        private static readonly BarcodeWriter<WriteableBitmap> writer = new BarcodeWriter<WriteableBitmap>
        {
            Options = new EncodingOptions
            {
                PureBarcode = true
            },
            Renderer = new WriteableBitmapRenderer(),
        };

        private void Refresh() => Refresh(double.IsNaN(Width) ? 300 : Convert.ToInt32(Width), double.IsNaN(Height) ? 150 : Convert.ToInt32(Height));

        private void Refresh(int width, int height)
        {
            if (string.IsNullOrEmpty(Value))
            {
                Source = null;
                return;
            }

            writer.Format = Format;
            writer.Options.Width = width;
            writer.Options.Height = height;

            Source = writer.Write(Value);
        }
    }
}
