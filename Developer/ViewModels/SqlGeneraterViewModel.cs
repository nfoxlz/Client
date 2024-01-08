using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace Compete.Mis.Developer.ViewModels
{
    internal sealed partial class SqlGeneraterViewModel : ViewModelBase
    {
        private readonly SqlGenerater.IGenerater generater = new SqlGenerater.PostgreSQLGenerater();

        [ObservableProperty]
        private string? _sqlText;

        [ObservableProperty]
        private IEnumerable<Models.DbTable>? _tables;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCreateSqlCommand))]
        private Models.DbTable? _table;

        private Models.ProjectSetting? projectSetting;

        public void SetProjectSetting(Models.ProjectSetting setting)
        {
            projectSetting = setting;

            Tables = projectSetting.Model.EntitySettings.Values;
        }

        private bool HasTable() => Table != null;

        [RelayCommand(CanExecute = nameof(HasTable))]
        private void GenerateCreateSql() => SqlText = generater.GenerateCreateSql(Table!);

        [RelayCommand]
        private void GenerateAll() => SqlText = generater.GenerateAll(Tables!);
    }
}
