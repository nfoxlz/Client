using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace Compete.Mis.Plugins
{
    public partial class PluginViewModel : ViewModels.ViewModelBase
    {
        public PluginCommandParameter? PluginParameter { get; set; }

        //[ObservableProperty]
        //private object? _pluginSetting;

        //private string? _pluginSetting

        //partial void OnPluginSettingChanged(object? value) => NewSetting(value);

        //protected virtual void NewSetting(object? setting) { }

        /// <summary>
        /// 获取或设置插件内部权限。
        /// </summary>
        public long Authorition { get; set; } = -1L;

        /// <summary>
        /// 检查是否有权限。
        /// </summary>
        /// <param name="authorition">要检查的权限。</param>
        /// <returns>true为有权限，false为无权限。</returns>
        protected bool HasAuthorition(long authorition) => (Authorition & authorition) != 0L;

        /// <summary>
        /// 检查是否有权限。
        /// </summary>
        /// <param name="authorition">要检查的权限。</param>
        /// <returns>true为有权限，false为无权限。</returns>
        protected bool HasAuthorition(ReserveAuthorition authorition) => HasAuthorition((long)authorition);

        /// <summary>
        /// 检查是否有可视权限。
        /// </summary>
        /// <param name="authorition">要检查的权限。</param>
        /// <returns>true为有权限，false为无权限。</returns>
        protected Visibility HasVisibleAuthorition(long authorition) => HasAuthorition(authorition) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// 检查是否有可视权限。
        /// </summary>
        /// <param name="authorition">要检查的权限。</param>
        /// <returns>true为有权限，false为无权限。</returns>
        protected Visibility HasVisibleAuthorition(ReserveAuthorition authorition) => HasAuthorition(authorition) ? Visibility.Visible : Visibility.Collapsed;

        protected virtual PluginCommandParameter GetRunParameter(PluginCommandParameter parameter) => parameter;

        protected virtual Action<bool>? BackAction { get; }

        [RelayCommand(CanExecute = nameof(CanRun))]
        private void Run(PluginCommandParameter parameter)
        {
            var commandParameter = GetRunParameter(parameter);
            commandParameter.Authorition &= Authorition;
            commandParameter.BackAction = BackAction;
            var command = PluginHelper.CreateCommand(commandParameter)!;
            command.Execute(parameter);
        }

        public bool HasRunAuthorition { get => HasAuthorition(ReserveAuthorition.Run); }

        protected virtual bool CanRun(PluginCommandParameter parameter) => HasRunAuthorition;
    }
}
