// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/18 13:17:47 LeeZheng 新建。
//==============================================================
using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace Compete.Mis.Developer.PowerDesigner
{
    /// <summary>
    /// PowerDesigner 助手类。
    /// </summary>
    internal static class PowerDesignerHelper
    {
        public static readonly string[] UnvisibleColumns = ["Id", "No", "Is_Deleted", "Deleter_User_Id", "Deleter_User_Id", "Deletion_Date_Time", "Version", "Tenant_Id", "Bill_Id", "Bill_Detail_Id", "User_Password", "Is_Active"];//, "Tenant_Id"

        public static readonly string[] ReadOnlyColumns =
        [
            "Creator_User_Id", "Creation_Date_Time", "Last_Modifier_User_Id", "Last_Modification_Date_Time",
            "Current_No",
            "Last_Purchase_Price", "Available_Inventory_Quantity", "Available_Inventory_Piece", "Inventory_Quantity", "Inventory_Piece", "Inventory_Cost", "Cost_Price",
            "AR_Amount", "AP_Amount", "PR_Amount", "PP_Amount",
            "Bill_Date", "In_Bill_Detail_Id", "Out_Bill_Detail_Id"
        ];

        public static void Export(string powerDesignerPath, Models.DataModel model, ExportMode mode = ExportMode.Merge)
        {
            if (!File.Exists(powerDesignerPath))
                throw new FileNotFoundException(string.Format("【{0}】文件不存在。", powerDesignerPath));

            if (mode == ExportMode.Override)
            {
                model.EntitySettings.Clear();
                model.ColumnSettings.Clear();
            }

            var xmlDocument = new XmlDocument();
            using FileStream fileStream = File.Open(powerDesignerPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            try
            {
                xmlDocument.Load(fileStream);

                var namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                namespaceManager.AddNamespace("o", "object");
                namespaceManager.AddNamespace("c", "collection");
                namespaceManager.AddNamespace("a", "attribute");

                // 导入表
                ExportModel(xmlDocument, namespaceManager, "Table", "Column", model/*, mode*/);
                // 导入视图
                ExportModel(xmlDocument, namespaceManager, "View", "ViewColumn", model/*, mode*/);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }
        }

        /// <summary>
        /// 导入模型设置。
        /// </summary>
        /// <param name="data">要存储模型的数据集。</param>
        /// <param name="xmlDocument">存储模型的XML文档。</param>
        /// <param name="namespaceManager">命令空间管理器。</param>
        /// <param name="modelType">模型类型。（表：Table；视图：View。）</param>
        /// <param name="columnType">列类型。（列：Column；视图列：ViewColumn。）</param>
        private static void ExportModel(XmlDocument xmlDocument, XmlNamespaceManager namespaceManager, string modelType, string columnType, Models.DataModel model/*, ExportMode mode*/)
        {

            IDataTypeConverter converter = GetConverter(xmlDocument.SelectNodes("/Model/o:RootObject/c:Children/o:Model/c:DBMS/o:Shortcut/a:Name", namespaceManager)![0]!.InnerText);
            XmlNodeList columnXmlNodeList;
            XmlNodeList commentXmlNodeList;
            XmlNodeList keyXmlNodeList;
            XmlNodeList indexXmlNodeList;
            XmlNodeList indexColumnXmlNodeList;
            XmlNodeList tableXmlNodeList = xmlDocument.SelectNodes(string.Format("/Model/o:RootObject/c:Children/o:Model/c:{0}s/o:{0}", modelType), namespaceManager)!;
            bool isView = "VIEW".Equals(modelType.Trim(), StringComparison.OrdinalIgnoreCase);

            string tableCode;
            string columnCode;
            //ICollection<string> keys;
            //IDictionary<string, ICollection<string>> indexes;
            var objectDictionary = new Dictionary<string, string>();
            ObservableCollection<string> indexColumns;
            Models.DbTable tableInfo;
            MemoryData.DataColumnSetting columnInfo;
            foreach (XmlNode node in tableXmlNodeList)
            {
                tableCode = node.SelectNodes("a:Code", namespaceManager)![0]!.InnerText.Trim();

                if (model.EntitySettings.TryGetValue(tableCode, out Models.DbTable? value))
                    tableInfo = value;
                else
                {
                    tableInfo = new Models.DbTable
                    {
                        Code = tableCode,
                        Name = node.SelectNodes("a:Name", namespaceManager)![0]!.InnerText,
                        Comment = node.SelectNodes("a:Comment", namespaceManager)?[0]?.InnerText,
                        ColumnSettings = new ObservableCollection<MemoryData.DataColumnSetting>(),
                    };
                    model.EntitySettings.Add(tableCode, tableInfo);
                }

                columnXmlNodeList = node.SelectNodes(string.Format("c:Columns/o:{0}", columnType), namespaceManager)!;
                foreach (XmlNode columnNode in columnXmlNodeList)
                {
                    columnCode = columnNode.SelectNodes("a:Code", namespaceManager)![0]!.InnerText.Trim();
                    objectDictionary.Add(columnNode.Attributes!["Id"]!.Value, columnCode);

                    columnInfo = new MemoryData.DataColumnSetting
                    {
                        ColumnName = columnCode
                    };
                    //var columns = from column in model.ColumnSettings
                    //              where column.ColumnName == columnCode
                    //              select column;
                    //if (columns.LongCount() > 0L)
                    //{
                    //    columnInfo = columns.First();
                    //    //if (!tableInfo.Columns.ContainsKey(columnCode))
                    //    //    tableInfo.Columns.Add(columnCode, columnInfo);
                    //    tableInfo.ColumnSettings.Add(columnInfo);

                    //    if (mode != ExportMode.Merge)
                    //        continue;
                    //}
                    //else
                    //{
                    //    columnInfo = new Data.DataColumnSetting();
                    //    model.ColumnSettings.Add(columnInfo);
                    //    //if (!tableInfo.Columns.ContainsKey(columnCode))
                    //    //    tableInfo.Columns.Add(columnCode, columnInfo);
                    //    tableInfo.ColumnSettings.Add(columnInfo);
                    //}
                    commentXmlNodeList = columnNode.SelectNodes("a:Comment", namespaceManager)!;
                    if (commentXmlNodeList.Count > 0)
                        columnInfo.Comment = commentXmlNodeList[0]!.InnerText;

                    if (columnNode.SelectNodes("a:DefaultValue", namespaceManager)!.Count > 0)
                    {
                        columnInfo.DefaultValue = columnNode.SelectNodes("a:DefaultValue", namespaceManager)![0]!.InnerText.Trim();
                        if (columnInfo.DefaultValue.ToString() == converter.DefaultCurrentDateTime)
                        {
                            columnInfo.DefaultSystemValue = MemoryData.SystemVariables.CurrentDateTime;
                            columnInfo.DefaultValue = null;
                        }
                        else if (columnInfo.DefaultValue.ToString() == converter.DefaultCurrentDate)
                        {
                            columnInfo.DefaultSystemValue = MemoryData.SystemVariables.CurrentDate;
                            columnInfo.DefaultValue = null;
                        }
                        else if (columnInfo.DefaultValue.ToString() == "'\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0\\0'")
                            columnInfo.DefaultValue = null;
                    }

                    // 设置显示名。
                    columnInfo.Caption = columnNode.SelectNodes("a:Name", namespaceManager)![0]!.InnerText;
                    if (columnCode.EndsWith("_Id"))
                    {
                        int startIndex = columnInfo.Caption.IndexOf("内码");
                        if (startIndex > 0)
                            columnInfo.Caption = columnInfo.Caption.Remove(startIndex);
                    }

                    foreach (var column in UnvisibleColumns)
                        if (column == columnCode)
                        {
                            columnInfo.IsVisible = false;
                            break;
                        }

                    foreach (var column in ReadOnlyColumns)
                        if (column == columnCode)
                        {
                            columnInfo.IsReadOnly = true;
                            break;
                        }

                    // 设置列设置
                    // 设置数据类型。
                    if (columnNode.SelectNodes("a:DataType", namespaceManager)!.Count > 0)
                    {
                        columnInfo.DbPhysicalType = columnNode.SelectNodes("a:DataType", namespaceManager)![0]!.InnerText.Trim();
                        columnInfo.DbDataType = converter.DataTypeToDbType(columnInfo.DbPhysicalType);
                    }
                    else
                        columnInfo.DbDataType = DbType.String;

                    //columnInfo.DataType = columnInfo.DbDataType.ToType();

                    // 设置长度。
                    if (columnNode.SelectNodes("a:Length", namespaceManager)!.Count > 0)
                        columnInfo.MaxLength = Convert.ToInt32(columnNode.SelectNodes("a:Length", namespaceManager)![0]!.InnerText);
                    //{
                    //    var length = Convert.ToInt32(columnNode.SelectNodes("a:Length", namespaceManager)![0]!.InnerText);
                    //    if (columnInfo.DbDataType.IsNumeric())
                    //    {
                    //        columnInfo.Length = length;
                    //        columnInfo.MaxLength = -1;
                    //    }
                    //    else
                    //        columnInfo.MaxLength = length;
                    //}
                    else
                    {
                        columnInfo.MaxLength = columnInfo.DbDataType switch
                        {
                            DbType.Double => 15,
                            DbType.Currency => 20,
                            _ => -1,
                        };
                    }

                    // 设置精度。 
                    if (columnNode.SelectNodes("a:Precision", namespaceManager)!.Count > 0)
                        columnInfo.Precision = Convert.ToInt16(columnNode.SelectNodes("a:Precision", namespaceManager)![0]!.InnerText);
                    else
                    {
                        columnInfo.Precision = columnInfo.DbDataType switch
                        {
                            DbType.Currency => 2,
                            _ => (short)(columnInfo.ColumnName.EndsWith("Price") ? 2 : 0),
                        };
                    }

                    columnInfo.IsAutoIncrement = columnInfo.IsVisible && !columnInfo.IsReadOnly && columnNode.SelectNodes("a:Identity", namespaceManager)!.Count != 0 && columnNode.SelectNodes("a:Identity", namespaceManager)![0]!.InnerText == "1";

                    // 设置是否允许为空。
                    columnInfo.IsRequired = columnInfo.IsVisible && !columnInfo.IsReadOnly && columnNode.SelectNodes("a:Column.Mandatory", namespaceManager)!.Count != 0 && columnNode.SelectNodes("a:Column.Mandatory", namespaceManager)![0]!.InnerText == "1";
                    columnInfo.AllowDBNull = !columnInfo.IsRequired;

                    if (columnInfo.IsVisible)
                    {
                        //if (columnInfo.DbDataType == DbType.Byte)
                        //    columnInfo.EditControl = DataControlType.SinglechoiceBox;
                        //else if (columnInfo.Name.EndsWith("Id") && columnInfo.Name != "Id" && columnInfo.Name != "BillId")
                        //    columnInfo.Control = DataControlType.EntityBox;
                        //else
                        //    switch(columnInfo.DataType)
                        //    {
                        //        case DbType.Decimal:
                        //        case DbType.VarNumeric:
                        //        case DbType.Currency:
                        //            columnInfo.Maximum = decimal.MaxValue;
                        //            columnInfo.Minimum = decimal.MinValue;

                        //            if (columnInfo.Length > 0)
                        //            {
                        //                var max = (Convert.ToDecimal(Math.Pow(10, columnInfo.Length)) - 1M) / Convert.ToDecimal(Math.Pow(10, columnInfo.Precision));
                        //                if (max < (decimal)columnInfo.Maximum)
                        //                    columnInfo.Maximum = max;
                        //                if (-max > (decimal)columnInfo.Minimum)
                        //                    columnInfo.Minimum = -max;
                        //            }
                        //            break;
                        //        case DbType.Int64:
                        //            columnInfo.Maximum = long.MaxValue;
                        //            columnInfo.Minimum = long.MinValue;
                        //            break;
                        //        case DbType.Int32:
                        //            columnInfo.Maximum = int.MaxValue;
                        //            columnInfo.Minimum = int.MinValue;
                        //            break;
                        //        case DbType.Int16:
                        //            columnInfo.Maximum = short.MaxValue;
                        //            columnInfo.Minimum = short.MinValue;
                        //            break;
                        //        case DbType.SByte:
                        //            columnInfo.Maximum = sbyte.MaxValue;
                        //            columnInfo.Minimum = sbyte.MinValue;
                        //            break;
                        //        case DbType.UInt64:
                        //            columnInfo.Maximum = ulong.MaxValue;
                        //            columnInfo.Minimum = ulong.MinValue;
                        //            break;
                        //        case DbType.UInt32:
                        //            columnInfo.Maximum = uint.MaxValue;
                        //            columnInfo.Minimum = uint.MinValue;
                        //            break;
                        //        case DbType.UInt16:
                        //            columnInfo.Maximum = ushort.MaxValue;
                        //            columnInfo.Minimum = ushort.MinValue;
                        //            break;
                        //        case DbType.Double:
                        //            columnInfo.Maximum = double.MaxValue;
                        //            columnInfo.Minimum = double.MinValue;
                        //            break;
                        //        case DbType.Single:
                        //            columnInfo.Maximum = float.MaxValue;
                        //            columnInfo.Minimum = float.MinValue;
                        //            break;
                        //        case DbType.DateTime:
                        //        case DbType.DateTime2:
                        //        case DbType.Date:
                        //            columnInfo.Maximum = DateTime.MaxValue;
                        //            columnInfo.Minimum = DateTime.MinValue;
                        //            break;
                        //        case DbType.DateTimeOffset:
                        //            columnInfo.Maximum = DateTimeOffset.MaxValue;
                        //            columnInfo.Minimum = DateTimeOffset.MinValue;
                        //            break;
                        //        case DbType.Time:
                        //            columnInfo.Maximum = TimeSpan.MaxValue;
                        //            columnInfo.Minimum = TimeSpan.MinValue;
                        //            break;
                        //    }

                        if (columnInfo.ColumnName.EndsWith("Price") || columnInfo.ColumnName.EndsWith("Tax_Rate"))
                        {
                            columnInfo.Regex = "^\\d+(\\.\\d+)?$";
                            columnInfo.ErrorText = "【{0}】不能为负数。";
                            columnInfo.MinValue = decimal.Zero;
                        }
                        else if (columnInfo.ColumnName.EndsWith("Packing_Size"))
                        {
                            columnInfo.ErrorText = "【{0}】不能小于1。";
                            columnInfo.MinValue = decimal.One;
                        }

                        if (columnInfo.DbDataType == DbType.Currency)
                            columnInfo.Format = "C2";
                        else if (columnInfo.ColumnName.EndsWith("Rate"))
                            columnInfo.Format = "P";
                    }

                    tableInfo.ColumnSettings.Add(columnInfo);
                    //Data.DataColumnSetting columnCopyInfo = new Data.DataColumnSetting();
                    //columnInfo.CopyTo();
                    if (!(from column in model.ColumnSettings
                          where column.ColumnName.Equals(columnCode, StringComparison.OrdinalIgnoreCase)
                          select column).Any())
                        model.ColumnSettings.Add(columnInfo);
                    //if (!tableInfo.Columns.ContainsKey(columnCode))
                    //    tableInfo.Columns.Add(columnCode, columnInfo);
                }

                if (modelType == "View")
                {
                    tableInfo.IsView = true;
                    continue;
                }

                keyXmlNodeList = node.SelectNodes("c:Keys/o:Key/c:Key.Columns/o:Column", namespaceManager)!;
                tableInfo.KeySettings = new ObservableCollection<string>();
                foreach (XmlNode keyNode in keyXmlNodeList)
                    tableInfo.KeySettings.Add(objectDictionary[keyNode.Attributes!["Ref"]!.Value]);

                indexXmlNodeList = node.SelectNodes("c:Indexes/o:Index", namespaceManager)!;
                tableInfo.IndexSettings = new Dictionary<string, ICollection<string>>();
                foreach (XmlNode indexNode in indexXmlNodeList)
                {
                    indexColumnXmlNodeList = indexNode.SelectNodes("c:IndexColumns/o:IndexColumn/c:Column/o:Column", namespaceManager)!;
                    indexColumns = [];
                    foreach (XmlNode indexColumnNode in indexColumnXmlNodeList)
                        indexColumns.Add(objectDictionary[indexColumnNode.Attributes!["Ref"]!.Value]);
                    tableInfo.IndexSettings.Add(indexNode.SelectNodes("a:Code", namespaceManager)![0]!.InnerText.Trim(), indexColumns);
                }
            }
        }

        /// <summary>
        /// 取得数据类型转换器。
        /// </summary>
        /// <param name="dbms">数据库管理系统名。</param>
        /// <returns>数据类型转换器。</returns>
        private static IDataTypeConverter GetConverter(string dbms)
        {
            return dbms switch
            {
                "SQLServer" => new SQLServerConverter(),
                "MySQL 5.0" => new MySQLConverter(),
                "PostgreSQL 9.x" => new PostgreSQLConverter(),
                _ => new PostgreSQLConverter(),
            };
        }
    }
}
