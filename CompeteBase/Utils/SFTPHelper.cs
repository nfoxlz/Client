using Compete.Mis;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace Compete.Utils
{
    public sealed class SFTPHelper
    {
        public static Common.Result DownloadFile(string host, int port, string userName, string password, string remoteFilePath, string localFilePath)
        {
            var result = new Common.Result();

            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 连接服务器
                    client.Connect();
                    if (client.Exists(remoteFilePath))
                    {
                        // 创建本地目录（如果不存在）
                        _ = Directory.CreateDirectory(Path.GetDirectoryName(localFilePath)!);

                        // 下载文件
                        using (var fileStream = File.Create(localFilePath))
                        {
                            try
                            {
                                client.DownloadFile(remoteFilePath, fileStream);
                                result.ErrorNo = 0;
                                result.Message = $"文件下载成功: {localFilePath}";
                            }
                            catch (Exception ex)
                            {
                                result.ErrorNo = -1;
                                result.Message = $"下载文件时发生错误: {ex.Message}";
                            }
                            finally
                            {
                                fileStream.Close();
                            }
                        }
                    }
                    else
                        result.Message = "远程文件不存在";
                }
                catch (Exception ex)
                {
                    result.Message = $"发生错误: {ex.Message}";
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }

            return result;
        }

        public static Common.Result DownloadFiles(string host, int port, string userName, string password, string remoteBaseDirectory, IEnumerable<string> remoteFilePath, string localDirectory, Action<int, string>? action = null)
        {
            var result = new Common.Result();

            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 创建本地目录（如果不存在）
                    _ = Directory.CreateDirectory(Path.GetDirectoryName(localDirectory)!);

                    // 连接服务器
                    client.Connect();

                    int index = 0;
                    string localPath;
                    foreach (var fileName in remoteFilePath)
                    {
                        var remotePath = remoteBaseDirectory + "/" + fileName;
                        // 确保远程目录存在
                        if (client.Exists(remotePath))
                        {
                            localPath = Path.Combine(localDirectory, fileName);

                            // 创建本地目录（如果不存在）
                            _ = Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

                            // 下载文件
                            using (var fileStream = File.Create(localPath))
                                try
                                {
                                    client.DownloadFile(remotePath, fileStream);
                                }
                                catch (Exception ex)
                                {
                                    result.ErrorNo = -1;
                                    result.Message = $"下载文件时发生错误: {ex.Message}";
                                }
                                finally
                                {
                                    fileStream.Close();
                                }

                            if (action is not null)
                            {
                                index++;
                                action(index, fileName);
                            }
                        }
                        else
                        {
                            result.ErrorNo = -1;
                            result.Message = "远程文件不存在";
                            return result;
                        }
                    }

                    result.Message = $"文件下载成功。";
                }
                catch (Exception ex)
                {
                    result.ErrorNo = -2;
                    result.Message = $"发生错误: {ex.Message}";
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }

            return result;
        }

        public static Common.Result UploadFile(string host, int port, string userName, string password, string localFilePath, string remoteFilePath)
        {
            var result = new Common.Result();
            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 连接服务器
                    client.Connect();
                    if (File.Exists(localFilePath))
                    {
                        // 确保远程目录存在
                        var remoteDir = Path.GetDirectoryName(remoteFilePath);
                        if (!string.IsNullOrEmpty(remoteDir) && !client.Exists(remoteDir))
                            client.CreateDirectory(remoteDir);

                        // 上传文件
                        using (var fileStream = File.OpenRead(localFilePath))
                        {
                            try
                            {
                                client.UploadFile(fileStream, remoteFilePath);
                                result.ErrorNo = 0;
                                result.Message = $"文件上传成功: {remoteFilePath}";
                            }
                            catch (Exception ex)
                            {
                                result.ErrorNo = -1;
                                result.Message = $"上传文件时发生错误: {ex.Message}";
                            }
                            finally
                            {
                                fileStream.Close();
                            }
                        }
                    }
                    else
                    {
                        result.Message = "本地文件不存在";
                    }
                }
                catch (Exception ex)
                {
                    result.Message = $"发生错误: {ex.Message}";
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }

            // 返回结果
            return result;
        }

        public static Common.Result UploadFiles(string host, int port, string userName, string password, string localBasePath, IEnumerable<string> localFilePath, string remoteFilePath)
        {
            var result = new Common.Result();
            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 连接服务器
                    client.Connect();

                    foreach (var fileName in localFilePath)
                    {
                        var localFileName = Path.Combine(localBasePath, fileName);
                        // 确保本地文件存在
                        if (File.Exists(localFileName))
                        {
                            // 确保远程目录存在
                            var remoteDir = Path.GetDirectoryName(remoteFilePath);
                            if (!string.IsNullOrEmpty(remoteDir) && !client.Exists(remoteDir))
                                client.CreateDirectory(remoteDir);

                            // 上传文件
                            using (var fileStream = File.OpenRead(localFileName))
                            {
                                try
                                {
                                    client.UploadFile(fileStream, remoteFilePath + "/" + fileName);
                                }
                                catch (Exception ex)
                                {
                                    result.ErrorNo = -1;
                                    result.Message = $"上传文件时发生错误: {ex.Message}";
                                    return result;
                                }
                                finally
                                {
                                    fileStream.Close();
                                }
                            }
                        }
                        else
                        {
                            result.ErrorNo = -1;
                            result.Message = "本地文件不存在";
                            return result;
                        }
                    }

                    result.Message = $"文件上传成功。";
                }
                catch (Exception ex)
                {
                    result.ErrorNo = -2;
                    result.Message = $"发生错误: {ex.Message}";
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }

            // 返回结果
            return result;
        }

        public static Common.Result RenameFile(string host, int port, string userName, string password, string oldPath, string newPath)
        {
            var result = new Common.Result();
            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 连接服务器
                    client.Connect();
                    if (client.Exists(oldPath) && !client.Exists(newPath))
                    {
                        client.RenameFile(oldPath, newPath);
                        result.ErrorNo = 0;
                        result.Message = $"文件重命名成功: {oldPath} - {newPath}";
                    }
                    else
                    {
                        result.ErrorNo = -1;
                        result.Message = $"源文件【{oldPath}】不存在或目标文件【{newPath}】已存在。";
                    }
                }
                catch (Exception ex)
                {
                    result.Message = $"发生错误: {ex.Message}";
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }
            return result;
        }

        public static Common.Result Delete(string host, int port, string userName, string password, string path)
        {
            var result = new Common.Result();
            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 连接服务器
                    client.Connect();
                    if (client.Exists(path))
                    {
                        client.Delete(path);
                        result.ErrorNo = 0;
                        result.Message = $"文件删除成功：{path}";
                    }
                    else
                    {
                        result.ErrorNo = -1;
                        result.Message = $"文件删除成功失败：{path}";
                    }
                }
                catch (Exception ex)
                {
                    result.Message = $"发生错误：{ex.Message}";
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }
            return result;
        }

        public static bool Exists(string host, int port, string userName, string password, string path)
        {
            // 创建 SFTP 客户端
            using (var client = new SftpClient(host, port, userName, password))
            {
                try
                {
                    // 连接服务器
                    client.Connect();
                    return client.Exists(path);
                }
                catch (Exception ex)
                {
                    using (var factory = GlobalCommon.CreateLoggerFactory())
                        factory.CreateLogger<SFTPHelper>().LogError(GlobalCommon.LogMessage, ex.ToString());
                    return false;
                }
                finally
                {
                    // 断开连接
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }
        }

        public SFTPSetting Setting { get; private set; }

        public SFTPHelper(SFTPSetting setting) => Setting = setting;

        public SFTPHelper(string host, int port, string userName, string password) =>
            Setting = new SFTPSetting
            {
                Host = host,
                Port = port,
                UserName = userName,
                Password = password,
            };

        public Common.Result DownloadFile(string remoteFilePath, string localFilePath) => DownloadFile(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, Setting.RemoteFilePath + "/" + remoteFilePath, localFilePath);

        public Common.Result DownloadFiles(string remoteBasePath, IEnumerable<string> remoteFilePath, string localFilePath, Action<int, string>? action = null) => DownloadFiles(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, Setting.RemoteFilePath + "/" + remoteBasePath, remoteFilePath, localFilePath, action);

        public Common.Result UploadFile(string localFilePath, string remoteFilePath) => UploadFile(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, localFilePath, Setting.RemoteFilePath + "/" + remoteFilePath);

        public Common.Result UploadFiles(string localBasePath, IEnumerable<string> localFilePath, string remoteFilePath) => UploadFiles(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, localBasePath, localFilePath, Setting.RemoteFilePath + "/" + remoteFilePath);

        public Common.Result RenameFile(string oldPath, string newPath) => RenameFile(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, Setting.RemoteFilePath + "/" + oldPath, Setting.RemoteFilePath + "/" + newPath);

        public Common.Result Delete(string path) => Delete(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, path);

        //public Common.Result LogicalDelete(string path) => RenameFile(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, Setting.RemoteFilePath + "/" + path, Setting.RemoteRecyclerFilePath + "/" + path);

        public bool Exists(string path) => Exists(Setting.Host, Setting.Port, Setting.UserName, Setting.Password, Setting.RemoteFilePath + "/" + path);
    }
}
