namespace Compete.Utils
{
    public sealed class SFTPSetting
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 22;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string RemoteFilePath { get; set; } = "/";

        public static SFTPSetting Default => new SFTPSetting();
    }
}
