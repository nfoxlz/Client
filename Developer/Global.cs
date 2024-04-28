using Npgsql;
using System.Data.Common;

namespace Compete.Mis.Developer
{
    internal static class Global
    {
        //public static T? CreateFromJson<T>(string path) => JsonSerializer.Deserialize<T>(File.ReadAllText(path));

        public static void Initialize()
        {
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
        }
    }
}
