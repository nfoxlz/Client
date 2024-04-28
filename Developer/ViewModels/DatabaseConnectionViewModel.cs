using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using System.Data.Common;
using System.Windows;

namespace Compete.Mis.Developer.ViewModels
{
    internal sealed partial class DatabaseConnectionViewModel : ViewModelBase
    {
        public Models.DatabaseConnectionSetting ConnectionSetting { get; set; } = new Models.DatabaseConnectionSetting();

        [RelayCommand(CanExecute = nameof(CanTest))]
        private void Test()
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(ConnectionSetting.ProviderName!);
                using IDbConnection connection = factory.CreateConnection()!;
                // 设置连接字符串
                connection.ConnectionString = ConnectionSetting.ConnectionString;

                // 打开连接
                connection.Open();

                // 关闭连接
                connection.Close();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Xceed.Wpf.Toolkit.MessageBox.Show("The connection is successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanTest() => !string.IsNullOrWhiteSpace(ConnectionSetting.ProviderName) && !string.IsNullOrWhiteSpace(ConnectionSetting.ConnectionString);

        [RelayCommand(CanExecute = nameof(CanTest))]
        private void Ok(Window window) => window.DialogResult = true;
    }
}
