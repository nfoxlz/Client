using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Compete.Documents
{
    public static class ExcelExporter
    {
        /// <summary>
        /// 导出数据到Excel文件。
        /// </summary>
        /// <param name="data">要导出的数据。</param>
        /// <param name="filePath">保存Excel文件的路径。</param>
        public static void Export(DataGrid dataGrid, string filePath, string sheetName = "Data")
        {
            // 创建工作簿
            using (IWorkbook workbook = Path.GetExtension(filePath) == ".xls" ? new HSSFWorkbook() : new XSSFWorkbook())
            {
                ISheet sheet = workbook.CreateSheet(sheetName);

                // 创建表头行
                IRow headerRow = sheet.CreateRow(0);
                //for (int i = 0; i < dataGrid.Columns.Count; i++)
                //    headerRow.CreateCell(i).SetCellValue(dataGrid.Columns[i].Header.ToString());
                int index = 0;
                foreach (var column in dataGrid.Columns)
                    if (column.Visibility == Visibility.Visible)
                        headerRow.CreateCell(index++).SetCellValue(column.Header.ToString());

                // 填充数据
                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    index = 0;
                    IRow row = sheet.CreateRow(i + 1);
                    //for (int j = 0; j < dataGrid.Columns.Count; j++)
                    //    row.CreateCell(j).SetCellValue((dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock)?.Text ?? string.Empty);
                    foreach (var column in dataGrid.Columns)
                        if (column.Visibility == Visibility.Visible)
                            row.CreateCell(index++).SetCellValue((column.GetCellContent(dataGrid.Items[i]) as TextBlock)?.Text ?? string.Empty);
                }

                // 自动调整列宽
                for (int i = 0; i < dataGrid.Columns.Count; i++)
                    sheet.AutoSizeColumn(i);

                // 保存文件
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    try
                    {
                        workbook.Write(stream);
                    }
                    finally
                    {
                        stream.Close();
                    }

                workbook.Close();
            }
        }
    }
}
