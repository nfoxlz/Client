namespace Compete.Threading
{
    internal interface IApplicationManager
    {
        void DoEvents();

        public static IApplicationManager DefaultManager { get; } = new WpfApplicationManager();
    }
}
