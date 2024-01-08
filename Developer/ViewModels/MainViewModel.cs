using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Compete.Mis.Developer.ViewModels
{
    internal sealed partial class MainViewModel : ViewModelBase
    {
        private const string projectFilter = "JSON 文件|*.json|全部文件|*.*";

        private readonly Services.IProjectService service = new Services.FileSystem.ProjectService();

        private bool hasChanged = false;

        [ObservableProperty]
        private ICollection<Models.ErrorInfo> _errorList = new ObservableCollection<Models.ErrorInfo>();


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ImportDataModelCommand), nameof(ShowDataModelCommand), nameof(SqlGeneraterCommand))]
        private Models.ProjectSetting? _projectSetting;

        [ObservableProperty]
        private string? _projectPath;


        [RelayCommand]
        private void NewProject()
        {
            var dialog = new SaveFileDialog { FileName = "project.json", Filter = projectFilter };

            if (dialog.ShowDialog() == true)
            {
                ProjectSetting = service.New(dialog.FileName);
                ProjectPath = dialog.FileName;
                //Global.ColumnInfos.Clear();

                ((Views.MainWindow)Application.Current.MainWindow).mainDocumentPane.Children.Clear();
            }
        }

        [RelayCommand]
        private void OpenProject()
        {
            var dialog = new OpenFileDialog { Filter = projectFilter }; ;

            if (dialog.ShowDialog() == true)
            {
                ProjectSetting = service.Open(dialog.FileName);

                ProjectPath = dialog.FileName;

                ((Views.MainWindow)Application.Current.MainWindow).mainDocumentPane.Children.Clear();

                //RefreshPlugins();

                //pluginWatcher.Path = Path.Combine(Path.GetDirectoryName(service.ProjectPath), "plugins");
                //pluginWatcher.EnableRaisingEvents = true;
            }
        }

        [RelayCommand]
        private void SaveProject()
        {
            service.Save(ProjectPath!, ProjectSetting!);
            hasChanged = false;
        }

        [RelayCommand]
        private void SaveAsProject()
        {
            var dialog = new SaveFileDialog { FileName = ProjectPath, Filter = projectFilter };
            if (dialog.ShowDialog() == true)
            {
                ProjectPath = dialog.FileName;
                SaveProject();
            }
        }

        [RelayCommand]
        private void Build()
        {
            if (hasChanged)
                SaveProject();

            service.Build(ProjectPath!, ProjectSetting!, ErrorList);
        }

        /// <summary>
        /// 显示文档。
        /// </summary>
        /// <typeparam name="T">文档内容类型。</typeparam>
        /// <param name="title">文档标题。</param>
        /// <returns></returns>
        private static T ShowDocument<T>(string title)
        {
            LayoutContent? document = null;
            var pane = ((Views.MainWindow)Application.Current.MainWindow).mainDocumentPane;
            foreach (var child in pane.Children)
                if (child.Title == title)
                {
                    document = child;
                    break;
                }

            if (document == null)
            {
                document = new LayoutDocument() { Title = title, Content = Activator.CreateInstance<T>() };
                pane.Children.Add(document);                                            // 添加新的文档。
            }

            pane.SelectedContentIndex = pane.Children.IndexOf(document);    // 将新的文档设为选中。

            return (T)document.Content!;
        }

        [RelayCommand(CanExecute = nameof(HasProjectSetting))]
        private void ImportDataModel()
        {
            var dialog = new OpenFileDialog { Filter = "PowerDesigner文件|*.pdm|全部文件|*.*" };

            if (dialog.ShowDialog() == true)
            {
                //ProjectSetting.ColumnSettings.Clear();

                //if (Global.ProjectSetting.EntityInfos == null)
                //    Global.ProjectSetting.EntityInfos = new ObservableCollection<Models.DbTable>();
                //if (Global.ColumnInfos == null)
                //    Global.ColumnInfos = new ObservableCollection<Models.DbColumn>();
                PowerDesigner.PowerDesignerHelper.Export(dialog.FileName, ProjectSetting!.Model, PowerDesigner.ExportMode.Override);// , PowerDesigner.ExportMode.Override

                hasChanged = true;

                ShowDocument<DataGrid>("数据模型").ItemsSource = ProjectSetting.Model.ColumnSettings;
            }
        }

        [RelayCommand(CanExecute = nameof(HasProjectSetting))]
        private void ShowDataModel() => ShowDocument<DataGrid>("数据模型").ItemsSource = ProjectSetting!.Model.ColumnSettings;

        [RelayCommand(CanExecute = nameof(HasProjectSetting))]
        private void SqlGenerater()
        {
            var generater = ShowDocument<Views.SqlGenerater>("SQL生成器");
            ((SqlGeneraterViewModel)generater.DataContext).SetProjectSetting(ProjectSetting!);
        }

        private bool HasProjectSetting() => ProjectSetting != null;

        //[RelayCommand]
        //private void DataInitialize()
        //{

        //}
    }
}
