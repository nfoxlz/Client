using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Compete.Utils
{
    internal static class DataHelper
    {
        public static object? GetValue(object data, string key)
        {
            if (data is DataRow row)
                return row[key];
            else if (data is DataRowView rowView)
                return rowView[key];
            else
                return data.GetPropertyValue(key);
        }

        public static DataRow? GetRow(object data)
        {
            if (data is DataRow row)
                return row;
            else if (data is DataRowView rowView)
                return rowView.Row;
            else if (data is DataTable table)
                return table.Rows.Count > 0 ? table.Rows[0] : null;
            else if  (data is DataView view)
                return view.Count > 0 ? view[0].Row : null;
            else
                return null;
        }

        public static DataColumnCollection? GetColumns(object data)
        {
            if (data is DataTable table)
                return table.Columns;
            else if (data is DataView view)
                return view.Table!.Columns;
            else if (data is DataRowView rowView)
                return rowView.DataView.Table!.Columns;
            else if (data is DataRow row)
                return row.Table.Columns;
            else
                return null;
        }
    }
}
