using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compete.Mis.Frame.Services.Sftp
{
    internal sealed class UpdateService : IUpdateService
    {
        private const string fileListFileName = "FileList.csv";

        public Common.Result<IEnumerable<string>> Chcek(Utils.SFTPSetting setting)
        {
            var fileList = new List<string>();

            var tmpFile = Path.GetTempFileName();
            Utils.SFTPHelper helper = new Utils.SFTPHelper(setting);
            var downloadResult = helper.DownloadFile(fileListFileName, tmpFile);
            if (null != downloadResult && 0 == downloadResult.ErrorNo)
                try
                {
                    var remoteFileList = File.ReadAllLines(tmpFile, Encoding.UTF8);

                    var localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileListFileName);
                    if (File.Exists(localFilePath))
                    {
                        var localFileList = File.ReadAllLines(localFilePath, Encoding.UTF8);

                        // 比较本地和远程文件列表
                        foreach (var remoteFile in remoteFileList)
                        {
                            var remoteFileInfo = remoteFile.Split(',');
                            if (remoteFileInfo.Length < 3)
                                continue;
                            //Debug.Assert(remoteFileInfo.Length > 2, "长度必须大于2。");

                            foreach (var localFile in localFileList)
                            {
                                var localFileInfo = localFile.Split(',');
                                if (localFileInfo.Length < 3)
                                    continue;
                                //Debug.Assert(localFileInfo.Length > 2, "长度必须大于2。");

                                if (remoteFileInfo[0] == localFileInfo[0] && (remoteFileInfo[1] != localFileInfo[1] || int.Parse(remoteFileInfo[2]) > int.Parse(localFileInfo[2])))
                                {
                                    // 如果文件名和大小相同，则不需要更新
                                    remoteFileList = remoteFileList.Where(f => f != remoteFile).ToArray();
                                    fileList.Add(remoteFileInfo[0]);
                                    break;
                                }
                            }
                        }
                    }
                    else
                        // 如果本地文件列表不存在，则直接使用远程文件列表
                        fileList.AddRange(remoteFileList.Select(f => f.Split(',')[0]));
                }
                catch (Exception ex)
                {
                    // 处理异常，例如记录日志
                    using (var factory = GlobalCommon.CreateLoggerFactory())
                        factory.CreateLogger<UpdateService>().LogError($"Error reading file: {ex.Message}");

                    var result = new Common.Result<IEnumerable<string>>();
                    result.Message = ex.Message;
                    return result;
                }
                finally
                {
                    // 删除临时文件
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
                }
            else
            {
                // 处理下载错误，例如记录日志
                using (var factory = GlobalCommon.CreateLoggerFactory())
                    factory.CreateLogger<UpdateService>().LogError($"Error downloading file: {downloadResult?.Message}");

                var result = new Common.Result<IEnumerable<string>>();
                result.Message = downloadResult?.Message;
                return result;
            }

            return new Common.Result<IEnumerable<string>>(fileList);
        }
    }
}
