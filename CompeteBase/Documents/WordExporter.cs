using NPOI.XWPF.UserModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Compete.Documents
{
    public static class WordExporter
    {
        public static void Export(DataGrid dataGrid, string filePath, string title)
        {
            using (XWPFDocument doc = new XWPFDocument())
            {
                // 添加标题
                XWPFParagraph titlePara = doc.CreateParagraph();
                titlePara.Alignment = ParagraphAlignment.CENTER;
                XWPFRun titleRun = titlePara.CreateRun();
                titleRun.IsBold = true;
                titleRun.FontSize = 16;
                titleRun.SetText(title);

                // 创建表格
                XWPFTable table = doc.CreateTable(dataGrid.Items.Count + 1, dataGrid.Columns.Count);

                // 设置表头
                XWPFTableRow headerRow = table.GetRow(0);
                //for (int i = 0; i < dataGrid.Columns.Count; i++)
                //    headerRow.GetCell(i).SetText(dataGrid.Columns[i].Header.ToString());
                int index = 0;
                foreach (var column in dataGrid.Columns)
                    if (column.Visibility == Visibility.Visible)
                        headerRow.GetCell(index++).SetText(column.Header.ToString());

                // 填充数据
                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    index = 0;
                    XWPFTableRow row = table.GetRow(i + 1);
                    //for (int j = 0; j < dataGrid.Columns.Count; j++)
                    //    row.GetCell(j).SetText((dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock)?.Text ?? "");
                    foreach (var column in dataGrid.Columns)
                        if (column.Visibility == Visibility.Visible)
                            row.GetCell(index++).SetText((column.GetCellContent(dataGrid.Items[i]) as TextBlock)?.Text ?? string.Empty);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                    try
                    {
                        doc.Write(stream);
                    }
                    finally
                    {
                        stream.Close();
                    }

                doc.Close();
            }
        }
    }
}
