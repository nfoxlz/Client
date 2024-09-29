using System;
using System.Threading.Tasks;
using System.Windows;

namespace Compete.Threading
{
    public static class ThreadHelper
    {
        public static Task RunBackgroundUI(Action callback) => Task.Run(() => Application.Current.Dispatcher.Invoke(callback));

        public static Task<T> RunBackgroundUI<T>(Func<T> callback) => Task.Run(() => Application.Current.Dispatcher.Invoke(callback));
    }
}
