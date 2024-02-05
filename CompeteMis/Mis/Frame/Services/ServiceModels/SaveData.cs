namespace Compete.Mis.Frame.Services.ServiceModels
{
    public sealed class SaveData
    {
        public Models.SimpleData? AddedTable { get; set; }

        public Models.SimpleData? DeletedTable { get; set; }

        public Models.SimpleData? ModifiedTable { get; set; }

        public Models.SimpleData? ModifiedOriginalTable { get; set; }
    }
}
