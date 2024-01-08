// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/12 15:08:36 LeeZheng 新建。
//==============================================================
using System;
using System.Configuration;
using System.IO;

namespace Compete.Utils
{
    /// <summary>
    /// 路径助手类。
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// 将相对路径转换为绝对路径。对于绝对路径，保持不变。
        /// </summary>
        /// <param name="path">源始路径。</param>
        /// <returns>转换后路径。</returns>
        public static string Convert(params string[] paths)
        {
            var path = Path.Combine(paths);
            return path.IndexOf(Path.VolumeSeparatorChar) < 0 && path[0] != Path.DirectorySeparatorChar ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path) : path;
        }

        public static string PluginPath { get; private set; } = Convert(ConfigurationManager.AppSettings["PluginPath"] ?? "../plugins");

        public static string SettingPath { get; private set; } = Convert(ConfigurationManager.AppSettings["SettingPath"] ?? "../settings");
    }
}
