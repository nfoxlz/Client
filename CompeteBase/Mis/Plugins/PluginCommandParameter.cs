using System;
using System.Collections.Generic;

namespace Compete.Mis.Plugins
{
    public sealed class PluginCommandParameter : PluginParameter
    {
        public ReserveAuthorition CommandAuthorition { get; set; } = ReserveAuthorition.All;

        public required string Title { get; set; }

        public long Authorition { get; set; } = -1L;

        public IDictionary<string, object>? Data { get; set; }

        public Action<bool>? BackCallAction { get; set; }

        public bool RequiredCurrentItem { get; set; }
    }
}
