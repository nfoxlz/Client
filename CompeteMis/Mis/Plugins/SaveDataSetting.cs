using System.Collections.Generic;

namespace Compete.Mis.Plugins
{
    public sealed class SaveDataSetting
    {
        public string[]? AddedColumns { get; set; }

        public string[]? DeletedColumns { get; set; }

        public string[]? ModifiedColumns { get; set; }

        public string[]? ModifiedOriginalColumns { get; set; }
    }
}
