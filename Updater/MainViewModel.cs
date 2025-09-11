using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Compete.Updater
{
    internal sealed partial class MainViewModel : ObservableObject
    {
        private static readonly string pluginPath = ConfigurationManager.AppSettings["PluginsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../plugins");

        private static readonly string settingPath = ConfigurationManager.AppSettings["SettingsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings");

        private string[]? fileList;

        private Utils.SFTPSetting sftpSetting;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
        private int _fileCount;

        [ObservableProperty]
        private int _downloadProgress;

        [ObservableProperty]
        private string _downloadPrompt = string.Empty;

        [ObservableProperty]
        private int _copyProgress;

        [ObservableProperty]
        private string _copyPrompt = string.Empty;

        public MainViewModel()
        {
            var sftpSettingPath = Path.Combine(ConfigurationManager.AppSettings["SettingsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings"), "sftp.json");
            sftpSetting = File.Exists(sftpSettingPath) ? JsonSerializer.Deserialize<Utils.SFTPSetting>(File.ReadAllText(sftpSettingPath)) ?? Utils.SFTPSetting.Default : Utils.SFTPSetting.Default;

            //FileCount = 0;
            DownloadProgress = 0;
            CopyProgress = 0;

            try
            {
                using (var memoryMappedFile = MemoryMappedFile.OpenExisting("UpdateSharedMemory"))
                using (var accessor = memoryMappedFile.CreateViewAccessor())
                {
                    accessor.Read(0, out int length);

                    var bytes = new byte[length];
                    accessor.ReadArray(4, bytes, 0, length);

                    fileList = Encoding.UTF8.GetString(bytes).Split('|', StringSplitOptions.RemoveEmptyEntries);
                }
            }
            catch (FileNotFoundException /*fileNotFoundException*/)
            {
                // TODO: 处理异常
                //MessageBox.Show(fileNotFoundException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            FileCount = fileList == null ? 0 : fileList.Length;
        }

        private bool canUpdate = true;

        [RelayCommand(CanExecute = nameof(CanUpdate))]
        private void Update() => Task.Run(() =>
        {
            canUpdate = false;
            UpdateCommand.NotifyCanExecuteChanged();

            var tmpDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());//Path.GetTempFileName();

            try
            {
                _ = Directory.CreateDirectory(tmpDirectory);
                Utils.SFTPHelper helper = new Utils.SFTPHelper(sftpSetting);
                var downloadResult = helper.DownloadFiles(string.Empty, fileList!, tmpDirectory, (index, fileName) =>
                {
                    DownloadProgress = index;
                    DownloadPrompt = $"{Mis.GlobalCommon.GetMessage("Updater.DownloadPrompt")} {fileName} ({index}/{FileCount})";
                });

                DownloadPrompt = Mis.GlobalCommon.GetMessage("Updater.DownloadComplete");

                string localFilePath;
                if (downloadResult != null && downloadResult.ErrorNo == 0)
                {
                    foreach (var file in fileList!)
                    {
                        CopyProgress++;
                        CopyPrompt = $"{Mis.GlobalCommon.GetMessage("Updater.CopyPrompt")} {file} ({CopyProgress}/{FileCount})";

                        if (file.StartsWith("bin\\", StringComparison.OrdinalIgnoreCase))
                            localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file[4..]);
                        else if (file.StartsWith("plugins\\", StringComparison.OrdinalIgnoreCase))
                            localFilePath = Path.Combine(pluginPath, file[8..]);
                        else if (file.StartsWith("settings\\", StringComparison.OrdinalIgnoreCase))
                            localFilePath = Path.Combine(settingPath, file[9..]);
                        else
                            continue;

                        var localDir = Path.GetDirectoryName(localFilePath);

                        try
                        {
                            if (!string.IsNullOrEmpty(localDir) && !Directory.Exists(localDir))
                                Directory.CreateDirectory(localDir);
                            File.Copy(Path.Combine(tmpDirectory, file), localFilePath, true);
                        }
                        catch (Exception ex)
                        {
                            Mis.MisControls.MessageDialog.Exception(ex);
                        }
                    }

                    DownloadPrompt = Mis.GlobalCommon.GetMessage("Updater.CopyComplete");
                }
            }
            finally
            {
                if (Directory.Exists(tmpDirectory))
                    Directory.Delete(tmpDirectory, true);
            }

            Mis.MisControls.MessageDialog.Success("Updater.UpdateComplete");

            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CompeteMis.exe"));

            Application.Current.Shutdown();
            Environment.Exit(0);

            //canUpdate = true;
            //UpdateCommand.NotifyCanExecuteChanged();
        });

        private bool CanUpdate() => canUpdate && fileList != null && fileList.Length > 0;
        //private bool CanUpdate()
        //{
        //    MessageBox.Show(fileList != null && fileList.Length > 0 ? "a" : "b");
        //    return fileList != null && fileList.Length > 0;
        //}
    }
}
