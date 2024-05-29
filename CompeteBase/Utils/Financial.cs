using System;
using System.Runtime.InteropServices;

namespace Compete.Utils
{
    public static class Financial
    {
        [DllImport("CompeteLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getLevelLen(string code, string structure);

        public static int GetLevelLen(string code)
        {
            var structure = Mis.GlobalCommon.GlobalConfiguration!.GetConfig<string>(ConfigurationNames.AccountStructure);
            if (string.IsNullOrWhiteSpace(structure))
                return code.Length;

            return getLevelLen(code, structure);
        }

        [DllImport("CompeteLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getNextLevelLen(string code, string structure);

        public static int GetNextLevelLen(string code)
        {
            var structure = Mis.GlobalCommon.GlobalConfiguration!.GetConfig<string>(ConfigurationNames.AccountStructure);
            if (string.IsNullOrWhiteSpace(structure))
                return code.Length + 1;

            return getNextLevelLen(code, structure);
        }

        //public static int GetLevelLen(string code)
        //{
        //    var structure = Mis.GlobalCommon.GlobalConfiguration!.GetConfig<string>(ConfigurationNames.AccountStructure);
        //    if (string.IsNullOrWhiteSpace(structure))
        //        return code.Length;

        //    var levels = structure.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        //    var codeLen = code.Length;
        //    int levelLen = 0, len = 0;
        //    foreach (var level in levels)
        //    {
        //        levelLen = Convert.ToInt32(level);
        //        len += levelLen;
        //        if (len >= codeLen)
        //            return levelLen;
        //    }

        //    return levelLen;
        //}

        //public static int GetNextLevelLen(string code)
        //{
        //    var structure = Mis.GlobalCommon.GlobalConfiguration!.GetConfig<string>(ConfigurationNames.AccountStructure);
        //    if (string.IsNullOrWhiteSpace(structure))
        //        return code.Length + 1;

        //    var levels = structure.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        //    var codeLen = code.Length;
        //    var len = 0;
        //    foreach (var level in levels)
        //    {
        //        var levelLen = Convert.ToInt32(level);
        //        len += levelLen;
        //        if (len > codeLen)
        //            return levelLen;
        //    }

        //    return -1;
        //}
    }
}
