using System.Windows.Media;

namespace Compete.Extensions
{
    public static class ColorExtensions
    {
        public static uint ARGB(this Color color) => (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);

        public static Color ToColor(this uint colorValue) => Color.FromArgb((byte)((colorValue >> 24) & 0xFF), (byte)((colorValue >> 16) & 0xFF), (byte)((colorValue >> 8) & 0xFF), (byte)(colorValue & 0xFF));
    }
}
