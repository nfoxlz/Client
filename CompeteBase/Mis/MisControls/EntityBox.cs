using Compete.Extensions;
using System.Data;
using System.Linq;

namespace Compete.Mis.MisControls
{
    public class EntityBox : AbstractEntityBox
    {
        protected override string? GetDisplay(DataTable entities)
            => string.IsNullOrWhiteSpace(Format) || null == formatMethod ? entities.Rows[0][DisplayPath].ToString() : formatMethod?.Invoke(null, [entities.Rows[0]])?.ToString();

        protected override object? SelectData()
        {
            var dialog = new EntitySelectDialog()
            {
                Title = GlobalCommon.GetMessage("EntitySelectDialog.Title", EntityName),
                DataContext = new EntitySelectViewModel
                {
                    IsRequired = IsRequired,
                    ServiceParameter = ServiceParameter,
                    Conditions = GetSourceRow()?.ToDictionary(),
                },
            };

            var viewModel = (EntitySelectViewModel)dialog.DataContext;
            //viewModel.Conditions = GetSourceRow()?.ToDictionary();
            ////viewModel.FilterFormat = FilterFormat ?? "(Code LIKE '{0}%' OR Name LIKE '%{0}%' OR MnemonicCode LIKE '%{0}%' OR Barcode = '{0}')";
            //viewModel.ServiceParameter = ServiceParameter;
            //viewModel.IsRequired = IsRequired;
            viewModel.QueryData();

            _ = new DataGridDecorator(dialog.MainDataGrid);    // 生成DataGrid装饰器。

            return dialog.ShowDialog() == true ? viewModel.SelectedItem : null;
        }
    }
}
