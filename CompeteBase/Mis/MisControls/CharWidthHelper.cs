// ======================================================
// XXX项目
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间            作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/4/15 周一 10:17:11 LeeZheng 新建。
// ======================================================
using System.Windows;
using System.Windows.Media;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// CharWidthHelper 类。
    /// </summary>
    internal static class CharWidthHelper
    {
        private static GlyphTypeface GetGlyphTypeface(FontFamily? fontFamily = null, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
        {
            fontFamily ??= new FontFamily("Microsoft YaHei UI");

            Typeface typeface = new(fontFamily, style, weight, stretch);
            typeface.TryGetGlyphTypeface(out var glyphTypeface);

            if (glyphTypeface.CharacterToGlyphMap.Count < 3000)
            {
                fontFamily = new FontFamily("SimSun");
                typeface = new Typeface(fontFamily, style, weight, stretch);
                typeface.TryGetGlyphTypeface(out glyphTypeface);
            }

            return glyphTypeface;
        }

        private static double GetCharWidth(char baseChar, int count, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
        {
            var glyphTypeface = GetGlyphTypeface(fontFamily, style, weight, stretch);
            return glyphTypeface.AdvanceWidths[glyphTypeface.CharacterToGlyphMap[baseChar]] * fontSize * count;
        }

        public static double GetStringWidth(string text, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
        {
            var glyphTypeface = GetGlyphTypeface(fontFamily, style, weight, stretch);

            var width = 0D;
            foreach (char nameChar in text)
                width += glyphTypeface.AdvanceWidths[glyphTypeface.CharacterToGlyphMap[nameChar]] * fontSize;

            return width;
        }

        public static double GetStringWidth(CharType charType, int count, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
            => charType switch
            {
                CharType.Chinese => GetChineseWidth(count, fontFamily, fontSize, style, weight, stretch),
                CharType.Number => GetNumberWidth(count, fontFamily, fontSize, style, weight, stretch),
                CharType.SpecialCharacter => GetSpecialCharacterWidth(count, fontFamily, fontSize, style, weight, stretch),
                _ => GetEnglishWidth(count, fontFamily, fontSize, style, weight, stretch),
            };

        public static double GetEnglishWidth(int count, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
            => GetCharWidth('W', count, fontFamily, fontSize, style, weight, stretch);

        public static double GetChineseWidth(int count, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
            => GetCharWidth('字', count, fontFamily, fontSize, style, weight, stretch);

        public static double GetNumberWidth(int count, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
            => GetCharWidth('8', count, fontFamily, fontSize, style, weight, stretch);

        public static double GetSpecialCharacterWidth(int count, FontFamily? fontFamily = null, double fontSize = 12D, FontStyle style = default, FontWeight weight = default, FontStretch stretch = default)
            => GetCharWidth('&', count, fontFamily, fontSize, style, weight, stretch);
    }
}
