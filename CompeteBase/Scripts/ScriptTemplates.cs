﻿// ======================================================
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
        public static IEnumerable<string> DataReferencedAssemblies { get; private set; } = ["System.dll", "System.Core.dll", "System.Data.dll"];

        public const string DataColumnChangingTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

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

        public const string FormatTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

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

        public const string VerifierTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

namespace Compete.Scripts
{{{{
    public static class {{0}}
    {{{{
        public static string {0}(DataSet data)
        {{{{
            {{1}}
        }}}}
    }}}}
}}}}";

        public const string BeforeTemplate = @"using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

namespace Compete.Scripts
{{{{
    public static class {{0}}
    {{{{
        public static bool {0}(object vm, DataSet? data = null)
        {{{{
            {{1}}
        }}}}
    }}}}
}}}}";

        public const string AfterTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

namespace Compete.Scripts
{{{{
    public static class {{0}}
    {{{{
        public static void {0}(object vm, DataSet? data = null)
        {{{{
            {{1}}
        }}}}
    }}}}
}}}}";

        public const string CheckTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

namespace Compete.Scripts
{{{{
    public static class {{0}}
    {{{{
        public static bool {0}(object? item)
        {{{{
            {{1}}
        }}}}
    }}}}
}}}}";

        public const string RunCheckTemplate = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Compete.Mis;
using Compete.Mis.Plugins;
using Compete.Mis.MisControls;

namespace Compete.Scripts
{{{{
    public static class {{0}}
    {{{{
        public static bool {0}(PluginCommandParameter parameter, object? item, IDictionary<string, object?> context)
        {{{{
            {{1}}
        }}}}
    }}}}
}}}}";

        public const string InitializingMethodClassName = "Compete.Scripts.InitializingClass";

        public const string InitializedMethodClassName = "Compete.Scripts.InitializedClass";

        public const string QueringMethodClassName = "Compete.Scripts.QueringClass";

        public const string QueriedMethodClassName = "Compete.Scripts.QueriedClass";

        public const string CheckMethodClassName = "Compete.Scripts.CheckClass";

        public const string RunCheckMethodClassName = "Compete.Scripts.RunCheckClass";
    }
}
