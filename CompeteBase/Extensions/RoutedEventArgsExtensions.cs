using System;
using System.Windows;
using System.Windows.Input;

namespace Compete.Extensions
{
    public static class RoutedEventArgsExtensions
    {
        public static ICommandSource? GetCommandSource(this RoutedEventArgs args) => args.OriginalSource as ICommandSource;

        public static object? GetParamater(this RoutedEventArgs args) => args.GetCommandSource()?.CommandParameter;

        public static void Run<T>(this RoutedEventArgs args, Action<T?> action)
        {
            var source = args.GetCommandSource();
            var element = source as UIElement;
            if (element != null)
                element.IsEnabled = false;
            try
            {
                action((T?)Convert.ChangeType(source?.CommandParameter, typeof(T)));
            }
            finally
            {
                if (element != null)
                    element.IsEnabled = true;
            }
        }
    }
}
