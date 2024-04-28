using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Data;
using System.Windows;

namespace Compete.Mis.Developer.ViewModels
{
    internal sealed partial class MnemonicCodeGeneraterViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private string? _tableName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private string? _idColumnName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private string? _nameColumnName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private string? _mnemonicCodeColumnName;

        [RelayCommand(CanExecute = nameof(CanGenerate))]
        private void Generate()
        {
            var data = Core.SqlHelper.ExecuteDataSet(databaseSetting!, (command) =>
            {
                command.CommandText = string.Format("SELECT {1}, {2} FROM {0}", TableName, IdColumnName, NameColumnName);
            });

            if (Core.SqlHelper.Execute(databaseSetting!, (connection, transaction) =>
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = string.Format("UPDATE {0} SET {2} = @mnemonicCode WHERE {1} = @id", TableName, IdColumnName, MnemonicCodeColumnName);
                Core.SqlHelper.AddParameter(command, "id", DbType.Int64);
                Core.SqlHelper.AddParameter(command, "mnemonicCode", DbType.String);

                int result = 0;
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Core.SqlHelper.SetParameter(command, "id", row[IdColumnName!]);
                    Core.SqlHelper.SetParameter(command, "mnemonicCode", Utils.Chinese.GetSpell(row[NameColumnName!].ToString()!));

                    result += command.ExecuteNonQuery();
                }

                return result;
            }) >= 0)
                Xceed.Wpf.Toolkit.MessageBox.Show("The mnemonic code is generated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                Xceed.Wpf.Toolkit.MessageBox.Show("The mnemonic code generation failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool CanGenerate() => !(string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(IdColumnName) || string.IsNullOrWhiteSpace(NameColumnName) || string.IsNullOrWhiteSpace(MnemonicCodeColumnName));

        private Models.DatabaseConnectionSetting? databaseSetting;

        public void SetDatabaseSetting(Models.DatabaseConnectionSetting setting) => databaseSetting = setting;
    }
}
