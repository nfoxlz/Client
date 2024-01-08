using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Compete.Mis.Developer
{
    internal static class Extensions
    {
        public static string ToJsonString(this object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), });

        public static void WriteJsonFile(this object obj, string path) => File.WriteAllText(path, obj.ToJsonString());
    }
}
