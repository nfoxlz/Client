using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
#if DEBUG || DEBUG_JAVA
using Microsoft.Win32;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using System.IO;
#endif

namespace Compete.Mis.Frame.ViewModels
{
    internal sealed partial class MainViewModel : PageViewModel, IRefresher
    {
        public ICollection<Control> Menus { get; set; } = new ObservableCollection<Control>();

        public bool CanGoBack { get; set; }

        [ObservableProperty]
        private string _currentUserName;

        public MainViewModel()
        {
            CurrentUserName = GlobalCommon.CurrentUser!.Name!;
            GlobalCommon.Refresher = this;
        }

        private Control CreateMenu(Services.ServiceModels.MenuSetting setting, IEnumerable<Services.ServiceModels.MenuSetting> menuSettings)
        {
            

            if (setting.DisplayName == "-")
                return new Separator();

            var item = new MenuItem()
            {
                Header = setting.DisplayName,
                ToolTip = string.IsNullOrWhiteSpace(setting.ToolTip) ? null : setting.ToolTip,
                Tag = setting.PluginSetting,
            };

            if (string.IsNullOrWhiteSpace(setting.PluginSetting))
                CreateMenus(item, menuSettings, setting.MenuNo);
            else
            {
                var index = setting.DisplayName.IndexOf('(');
                var parameter = new Plugins.PluginCommandParameter()
                {
                    Path = setting.PluginSetting,
                    Parameter = setting.PluginParameter,
                    Title = index > 0 ? setting.DisplayName[..index] : setting.DisplayName,
                    Authorition = setting.Authorition,
                };
                item.CommandParameter = parameter;
                item.Command = Plugins.PluginHelper.CreateCommand(parameter);
            }

            return item;
        }

        private void CreateMenus(ItemsControl control, IEnumerable<Services.ServiceModels.MenuSetting> menuSettings, long parentNo)
        {
            var settings = from setting in menuSettings
                           where setting.ParentMenuNo == parentNo
                           orderby setting.Sn
                           select setting;

            foreach (var setting in settings)
                control.Items.Add(CreateMenu(setting, menuSettings));
        }

        public override void Refresh()
        {
            CurrentUserName = GlobalCommon.CurrentUser!.Name!;

            Menus.Clear();

            var menuSettings = Services.GlobalServices.FrameService.GetMenus();
            var settings = from setting in menuSettings
                           where setting.ParentMenuNo == 0L
                           orderby setting.Sn
                           select setting;

            foreach (var setting in settings)
                Menus.Add(CreateMenu(setting, menuSettings));

            // 添加系统菜单。
            Menus.Add(new MenuItem()
            {
                Header = GlobalCommon.GetMessage("MainPage.SystemMenu"),
                ItemsSource = new Control[]
                {
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.ModifyPasswordMenu"),
                        Command = ModifyPasswordCommand
                    },
                    new Separator(),
#if DEBUG || DEBUG_JAVA
                    new MenuItem()
                    {
                        Header = "清理缓存",
                        Command = CleanUpCacheCommand
                    },
                    new MenuItem()
                    {
                        Header = "生成文档",
                        Command = GenerateDocumentCommand
                    },
                    new Separator(),
#endif
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.LogoutMenu"),
                        Command = LogoutCommand,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Logout.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.ExitMenu"),
                        Command = ExitCommand,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Exit.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    }
                }
            });

            // 添加编辑菜单。
            Menus.Add(new MenuItem()
            {
                Header = GlobalCommon.GetMessage("MainPage.EditMenu"),
                ItemsSource = new Control[]
                {
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.UndoMenu"),
                        ToolTip = GlobalCommon.GetMessage("MainPage.UndoTip"),
                        Command = ApplicationCommands.Undo,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Undo.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.RedoMenu"),
                        ToolTip = GlobalCommon.GetMessage("MainPage.RedoTip"),
                        Command = ApplicationCommands.Redo,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Redo.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                    new Separator(),
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.CutMenu"),
                        ToolTip = GlobalCommon.GetMessage("MainPage.CutTip"),
                        Command = ApplicationCommands.Cut,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Cut.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.CopyMenu"),
                        ToolTip = GlobalCommon.GetMessage("MainPage.CopyTip"),
                        Command = ApplicationCommands.Copy,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Copy.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.PasteMenu"),
                        ToolTip = GlobalCommon.GetMessage("MainPage.PasteTip"),
                        Command = ApplicationCommands.Paste,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Paste.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                }
            });

            // 添加帮助菜单。
            Menus.Add(new MenuItem()
            {
                Header = GlobalCommon.GetMessage("MainPage.HelpMenu"),
                ItemsSource = new Control[]
                {
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.ViewHelpMenu"),
                        Command = HelpCommand,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/Help.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    },
                    new MenuItem()
                    {
                        Header = GlobalCommon.GetMessage("MainPage.AboutMenu"),
                        Command = AboutCommand,
                        Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("/MainFrame;component/Images/SmallIcon.png", UriKind.Relative)),
                            Width = 16D,
                            Height = 16D
                        }
                    }
                }
            });
        }

#if DEBUG || DEBUG_JAVA
        [RelayCommand]
        private static void CleanUpCache() => Services.GlobalServices.FrameService.ClearCache();

        [RelayCommand]
        private void GenerateDocument()
        {
            var dialog = new SaveFileDialog { Filter = "Word 文档(*.docx)|*.docx|全部文件|*.*" };
            if (dialog.ShowDialog() != true)
                return;

            GlobalCommon.MainDocumentPane!.Children.Clear();

            var doc = new XWPFDocument();

            ExecuteMenus(Menus, ref doc);

            // 保存文档
            using (var stream = new FileStream(dialog.FileName, FileMode.Create))
                doc.Write(stream);

            MisControls.MessageDialog.Success("文件生成完毕。");
        }

        private void ExecuteMenus(IEnumerable<Control> menus, ref XWPFDocument doc, int level = 1)
        {
            foreach (var control in menus)
                if (control is MenuItem item)
                {
                    var header = item.Header.ToString()!;
                    if (header.StartsWith("系统") || header.StartsWith("编辑") || header.StartsWith("帮助"))
                        continue;

                    var titlePara = doc.CreateParagraph();
                    titlePara.Style = $"标题{level}"; // 继承模板的一级标题样式
                    titlePara.CreateRun().SetText(header);

                    var toolTip = item.ToolTip?.ToString();
                    if (!string.IsNullOrWhiteSpace(toolTip))
                        doc.CreateParagraph().CreateRun().SetText(toolTip);

                    if (item.Command is not null)
                    {
                        item.Command.Execute(item.CommandParameter);
                        if (GlobalCommon.MainDocumentPane!.Children.Count > 0)
                        {
                            var content = GlobalCommon.MainDocumentPane!.Children[0];

                            try
                            {
                                if (content.Content is FrameworkElement element)
                                {
                                    var imagePath = Path.ChangeExtension(Path.GetTempFileName(), "png");
                                    try
                                    {
                                        //var info = Document.Screenshot.SaveControl(element, imagePath, Document.ImageFormat.Png);
                                        var info = Document.Screenshot.SaveWindow(Application.Current.MainWindow, imagePath, Document.ImageFormat.Png);

                                        // 插入图片
                                        byte[] imageBytes = File.ReadAllBytes(imagePath);
                                        string picId = doc.AddPictureData(imageBytes, (int)PictureType.PNG);
                                        var imgPara = doc.CreateParagraph();
                                        var imgRun = imgPara.CreateRun();
                                        imgRun.AddPicture(new MemoryStream(imageBytes),
                                                         (int)PictureType.JPEG, imagePath,
                                                         Units.ToEMU(info.Bounds.Width), Units.ToEMU(info.Bounds.Height));
                                    }
                                    finally
                                    {
                                        File.Delete(imagePath);
                                    }
                                }
                            }
                            finally
                            {
                                content.Close();
                            }
                        }
                    }
                    else
                        ExecuteMenus(item.Items.Cast<Control>(), ref doc, level++);
                }
        }
#endif

        [RelayCommand]
        private static void About()
        {
            (new Views.AboutBox()).ShowDialog();
        }

        [RelayCommand]
        private static void ModifyPassword() => (new Views.ModifyPasswordBox()).ShowDialog();

        [RelayCommand]
        private void Logout()
        {
            var count = GlobalCommon.MainDocumentPane!.Children.Count;
            if (count > 1 && MisControls.MessageDialog.Question("MainViewModel.LogoutMessage", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            for (int index = count - 1; index > 0; index--)
                GlobalCommon.MainDocumentPane.Children.RemoveAt(index);

            CanGoBack = true;
            ((NavigationWindow)Application.Current.MainWindow).GoBack();
        }

        [RelayCommand]
        private static void Exit()
        {
            if (GlobalCommon.MainDocumentPane!.Children.Count > 1 && MisControls.MessageDialog.Question("MainViewModel.ExitMessage", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private static void Help()
        {

        }
    }
}
