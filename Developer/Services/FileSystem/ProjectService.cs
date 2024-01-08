// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/17 15:52:53 LeeZheng 新建。
//==============================================================
using Compete.MemoryData;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Compete.Mis.Developer.Services.FileSystem
{
    /// <summary>
    /// 项目服务类。
    /// </summary>
    internal sealed class ProjectService : IProjectService
    {
        //private readonly ISettingService settingService = new SettingService();

        //public string ProjectPath { get; set; }

        public Models.ProjectSetting New(string path)
        {
            var setting = new Models.ProjectSetting();

            var basePath = Path.GetDirectoryName(path)!;

            //Directory.CreateDirectory(Path.Combine(basePath, "output"));
            Directory.CreateDirectory(Path.Combine(basePath, "plugins"));
            Directory.CreateDirectory(Path.Combine(basePath, "settings"));

            setting.WriteJsonFile(path);

            //ProjectPath = path;

            return setting;
        }

        public Models.ProjectSetting Open(string path) => JsonSerializer.Deserialize<Models.ProjectSetting>(File.ReadAllText(path))!;

        public void Save(string path, Models.ProjectSetting setting) => setting.WriteJsonFile(path);

        public void Build(string path, Models.ProjectSetting setting, ICollection<Models.ErrorInfo> errorList)
        {
            errorList.Clear();

            var outputPath = Path.Combine(Path.GetDirectoryName(path)!, setting.OutputPath);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var binPath = Path.Combine(outputPath, "bin");
            if (!Directory.Exists(binPath))
                Directory.CreateDirectory(binPath);

            var pluginsPath = Path.Combine(outputPath, "plugins");
            if (!Directory.Exists(pluginsPath))
                Directory.CreateDirectory(pluginsPath);

            var settingsPath = Path.Combine(outputPath, "settings");
            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            var bilder = new StringBuilder();

            var tables = new Dictionary<string, string>();
            foreach (var info in setting.Model.EntitySettings)
                tables.Add(info.Key, info.Value.Name);
            File.WriteAllText(Path.Combine(settingsPath, "tables.json"), tables.ToJsonString());
            errorList.Add(new Models.ErrorInfo { Level = Models.ErrorLevel.Information, Location = "EntityInfos", Message = "表名设置生成成功。" });
            bilder.AppendLine("settings\\tables.json,1");

            string json = setting.Model.ColumnSettings.ToJsonString();
            json = json.Replace($"\"{nameof(DataColumnSetting.DataType)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.AllowDBNull)}\":true,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.ReadOnly)}\":false,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.IsUnique)}\":false,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.IsAutoIncrement)}\":false,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.AutoIncrementSeed)}\":1,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.AutoIncrementStep)}\":1,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.Expression)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.MaxLength)}\":-1,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.Precision)}\":0,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.DefaultValue)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.IsRequired)}\":false,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.IsReadOnly)}\":false,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.IsVisible)}\":true,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.Format)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.ShowFormat)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.DefaultSystemValue)}\":0,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.MaxValue)}\":79228162514264337593543950335,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.MaxValue)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.MinValue)}\":-79228162514264337593543950335,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.MinValue)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.EditControl)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.Regex)}\":null,", string.Empty);
            json = json.Replace($"\"{nameof(DataColumnSetting.ErrorText)}\":null,", string.Empty);
            json = json.Replace($"\",{nameof(DataColumnSetting.Control)}\":0", string.Empty);
            json = json.Replace(",}", "}");
            //json = json.Replace($"\"{nameof(DataColumnSetting.Parameters)}\":null,", string.Empty);
            File.WriteAllText(Path.Combine(settingsPath, "columns.json"), json);

            errorList.Add(new Models.ErrorInfo { Level = Models.ErrorLevel.Information, Location = "ColumnInfos", Message = "列设置生成成功。" });
            bilder.AppendLine("settings\\columns.json,1");

            File.WriteAllText(Path.Combine(binPath, "file_list.csv"), bilder.ToString());
            errorList.Add(new Models.ErrorInfo { Level = Models.ErrorLevel.Information, Location = "file_list.csv", Message = "文件列表生成成功。" });
        }
    }
}
