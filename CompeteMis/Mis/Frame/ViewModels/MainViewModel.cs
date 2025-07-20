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

namespace Compete.Mis.Frame.ViewModels
{
    internal sealed partial class MainViewModel : PageViewModel
    {
        public ICollection<Control> Menus { get; set; } = new ObservableCollection<Control>();

        public bool CanGoBack { get; set; }

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
#if DEBUG
                    new MenuItem()
                    {
                        Header = "清理缓存",
                        Command = CleanUpCacheCommand
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
                    }
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

#if DEBUG
        [RelayCommand]
        private static void CleanUpCache() => Services.GlobalServices.FrameService.ClearCache();
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
