// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/1/27 周日 9:09:51 LeeZheng 新建。
// ======================================================
using System.Collections.Generic;

namespace Compete.Scripts
{
    /// <summary>
    /// ScriptTemplates 类。
    /// </summary>
    public static class ScriptTemplates
    {
        public static IEnumerable<string> DataReferencedAssemblies { get; private set; } = new List<string> { "System.dll", "System.Core.dll", "System.Data.dll" };

        public const string DataColumnChangingTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Compete.Scripts
{{
    public static class DataColumnChanging
    {{
        public static void DataColumnChangeEventHandler(object sender, DataColumnChangeEventArgs e)
        {{
            {0}
        }}
    }}
}}";

//        public const string CalculatorTemplate = @"using System;
//using System.Data;

//namespace Compete.Scripts
//{{
//    public static class Calculator
//    {{
//        public static void Compute(ref DataRow row)
//        {{
//            {0};
//        }}
//    }}
//}}";
        public const string CalculatorTemplate = @"using System;
using System.Data;

namespace Compete.Scripts
{{
    public static class {0}
    {{
        {1}
    }}
}}";

        public const string CalculatorMethodTemplate = @"        public static void {0}(ref DataRow row)
        {{
            {1};
        }}";

        public const string VerifyTemplate = @"using System;
using System.Data;

namespace Compete.Scripts
{{
    public static class {0}
    {{
        {1}
    }}
}}";

        public const string VerifyMethodTemplate = @"        public static string {0}(DataRow row, object? proposedValue)
        {{
            {1};
        }}";

        public const string FormatTemplate = @"using System.Data;

namespace Compete.Scripts
{{
    public static class Formater
    {{
        public static string GetString(DataRow row)
        {{
            return {0};
        }}
    }}
}}";

        //        public const string FormatTemplate = @"namespace Compete.Scripts
        //{{
        //    public static class Formater
        //    {{
        //        public static string GetString()
        //        {{
        //            return string.Empty;
        //        }}
        //    }}
        //}}";

        public const string VerifyMethodClassName = "Compete.Scripts.SaveVerifier";

        public const string VerifyethodTemplate = @"using System;
using System.Data;

namespace Compete.Scripts
{{
    public static class SaveVerifier
    {{
        public static string Verify(DataSet data)
        {{
            {0}
        }}
    }}
}}";
    }
}
