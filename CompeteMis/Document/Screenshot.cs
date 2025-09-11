#if DEBUG || DEBUG_JAVA
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Compete.Document
{
    internal static class Screenshot
    {
        private static void SaveVisual(Visual visual, Rect bounds, string filePath, ImageFormat format, int dpi)
        {
            var renderTarget = new RenderTargetBitmap(
                (int)bounds.Width,
                (int)bounds.Height,
                dpi, dpi, PixelFormats.Pbgra32);

            renderTarget.Render(visual);

            BitmapEncoder encoder = format switch
            {
                ImageFormat.Jpeg => new JpegBitmapEncoder { QualityLevel = 90 },
                ImageFormat.Bmp => new BmpBitmapEncoder(),
                ImageFormat.Gif => new GifBitmapEncoder(),
                ImageFormat.Tiff => new TiffBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (var stream = new FileStream(filePath, FileMode.Create))
                encoder.Save(stream);
        }

        public static ImageInfo SaveWindow(Window window, string filePath, ImageFormat format, int dpi = 96)
        {
            var result = new ImageInfo();

            // 确保窗口已完成布局
            window.Dispatcher.Invoke(() =>
            {
                window.UpdateLayout();
                result.Bounds = new Rect(0, 0, window.ActualWidth, window.ActualHeight);
                SaveVisual(window, result.Bounds, filePath, format, dpi);
            }, DispatcherPriority.Render);

            return result;
        }

        public static ImageInfo SaveControl(FrameworkElement control, string filePath, ImageFormat format, int dpi = 96)
        {
            var result = new ImageInfo();

            control.Dispatcher.Invoke(() =>
            {
                // 强制布局计算
                control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                control.Arrange(new Rect(control.DesiredSize));

                result.Bounds = new Rect(0, 0, control.ActualWidth, control.ActualHeight);
                SaveVisual(control, result.Bounds, filePath, format, dpi);
            }, DispatcherPriority.Render);

            return result;
        }
    }
}
#endif
