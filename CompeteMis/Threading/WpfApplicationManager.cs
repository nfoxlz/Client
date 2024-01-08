using System.Windows.Threading;

namespace Compete.Threading
{
    internal sealed class WpfApplicationManager : IApplicationManager
    {
        public void DoEvents()
        {
            var frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (object? state) => (state as DispatcherFrame)!.Continue = false, frame);
            Dispatcher.PushFrame(frame);
        }
    }
}
