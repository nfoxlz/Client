namespace Compete.Mis.Plugins
{
    public enum NumberType
    {
        /// <summary>
        /// 小写数字（阿拉伯数字）。
        /// </summary>
        Number,
        /// <summary>
        /// 大写数字（中文汉字）。
        /// </summary>
        WordNumber,
        /// <summary>
        /// 大写金额（有封头，有元角分单位）。
        /// </summary>
        WordCurrency,
    }
}
