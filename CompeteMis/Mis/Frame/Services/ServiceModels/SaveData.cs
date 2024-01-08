namespace Compete.Mis.Frame.Services.ServiceModels
{
    public sealed class SaveData
    {
        public Models.SimpleDataTable? AddedTable { get; set; }

        public Models.SimpleDataTable? DeletedTable { get; set; }

        public Models.SimpleDataTable? ModifiedTable { get; set; }

        public Models.SimpleDataTable? ModifiedOriginalTable { get; set; }
    }
}
