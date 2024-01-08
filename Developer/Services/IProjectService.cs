// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/17 15:51:01 LeeZheng 新建。
//==============================================================
using System.Collections.Generic;

namespace Compete.Mis.Developer.Services
{
    /// <summary>
    /// 项目服务接口。
    /// </summary>
    internal interface IProjectService
    {
        Models.ProjectSetting New(string path);

        Models.ProjectSetting Open(string path);

        void Save(string path, Models.ProjectSetting setting);

        void Build(string path, Models.ProjectSetting setting, ICollection<Models.ErrorInfo> errorList);
    }
}
