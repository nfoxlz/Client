using Compete.Mis.MisThreading;

namespace Compete.Threading
{
    internal sealed class WpfApplicationManager : IApplicationManager
    {
        public void DoEvents() => ThreadingHelperBase.DoEvents();
    }
}
