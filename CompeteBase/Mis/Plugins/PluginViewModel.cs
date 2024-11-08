using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
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

        protected virtual bool CheckAuthorition(long authorition) => true;

        /// <summary>
        /// 检查是否有权限。
        /// </summary>
        /// <param name="authorition">要检查的权限。</param>
        /// <returns>true为有权限，false为无权限。</returns>
        protected bool HasAuthorition(long authorition) => (Authorition & authorition) != 0L && CheckAuthorition(authorition);

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

        protected virtual Action<bool>? BackCallAction { get; }

        [RelayCommand(CanExecute = nameof(CanRun))]
        private void Run(PluginCommandParameter parameter)
        {
            var commandParameter = GetRunParameter(parameter);
            commandParameter.Authorition &= Authorition;
            commandParameter.BackCallAction = BackCallAction;
            var command = PluginHelper.CreateCommand(commandParameter)!;
            command.Execute(parameter);
        }

        public bool HasRunAuthorition { get => HasAuthorition(ReserveAuthorition.Run); }

        protected virtual bool GetRunAuthorition(PluginCommandParameter? parameter) => true;

        protected virtual bool CanRun(PluginCommandParameter parameter) => (parameter == null || parameter.CommandAuthorition == ReserveAuthorition.All ? HasRunAuthorition : HasAuthorition(parameter.CommandAuthorition)) && GetRunAuthorition(parameter);

        [RelayCommand(CanExecute = nameof(CanQueryMessage))]
        private void QueryMessage(DataMessageCommandParameter parameter)
        {
            var data = GlobalCommon.DataProvider!.Query(parameter.Path, parameter.Parameter!);
            var rows = data.Tables[0].Rows;
            if (rows.Count == 0)
                return;

            var row = rows[0];
            IList<object> list = [];
            foreach (DataColumn column in data.Tables[0].Columns)
                list.Add(row[column]);

            MisControls.MessageDialog.Information(string.Format(parameter.MessageFormatString ?? "{0}", [.. list]));
        }

        public bool HasQueryMessageAuthorition { get => HasAuthorition(ReserveAuthorition.QueryMessage); }

        private bool CanQueryMessage(DataMessageCommandParameter parameter) => parameter != null && HasQueryMessageAuthorition;
    }
}
