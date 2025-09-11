using System.Collections.Generic;

namespace Compete.Mis.Frame.Services
{
    public interface IUpdateService
    {
        Common.Result<IEnumerable<string>> Chcek(Utils.SFTPSetting setting);
    }
}
