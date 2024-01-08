using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using System.Windows;

namespace Compete.Mis.Frame.ViewModels
{
    internal sealed partial class AboutViewModel : ObservableObject
    {
        [RelayCommand]
        private void Ok(Window window) => window.Close();

        #region 程序集特性访问器

        /// <summary>
        /// 取得程序信自定义特性。
        /// </summary>
        /// <typeparam name="T">自定义特性类型。</typeparam>
        /// <returns>自定义特性值。</returns>
        private T GetAttributes<T>() => (T)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false)[0];

        /// <summary>
        /// 获取程序集标题。
        /// </summary>
        public string AssemblyTitle => GetAttributes<AssemblyTitleAttribute>().Title;

        /// <summary>
        /// 获取程序集版本。
        /// </summary>
        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        /// <summary>
        /// 获取程序集描述。
        /// </summary>
        //public string AssemblyDescription => GetAttributes<AssemblyDescriptionAttribute>().Description;

        /// <summary>
        /// 获取程序集产品名。
        /// </summary>
        public string AssemblyProduct => GetAttributes<AssemblyProductAttribute>().Product;

        /// <summary>
        /// 获取程序集版权。
        /// </summary>
        //public string AssemblyCopyright => GetAttributes<AssemblyCopyrightAttribute>().Copyright;

        /// <summary>
        /// 获取程序集所属公司。
        /// </summary>
        public string AssemblyCompany => GetAttributes<AssemblyCompanyAttribute>().Company;

        #endregion // 程序集特性访问器
    }
}
