using System.Collections.Generic;

namespace Compete.Mis.Plugins
{
    public sealed class PluginCommandParameter
    {
        public required string Path { get; set; }

        public string? Parameter { get; set; }

        public required string Title { get; set; }

        public long Authorition { get; set; } = -1L;

        public IDictionary<string, object>? Data { get; set; }
    }
}
