namespace Compete.Mis.Plugins
{
    public abstract class PluginParameter
    {
        public required string Path { get; set; }

        public string? Parameter { get; set; }
    }
}
