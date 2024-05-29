using Compete.Extensions;
using System.Data;
using System.Windows;

namespace Compete.Mis.MisControls
{
    public class TreeEntityBox : AbstractEntityBox
    {
        public string? LevelLength
        {
            get { return (string?)GetValue(LevelLengthProperty); }
            set { SetValue(LevelLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelLengthProperty =
            DependencyProperty.Register(nameof(LevelLength), typeof(string), typeof(TreeEntityBox));

        public string? LevelPath
        {
            get { return (string?)GetValue(LevelPathProperty); }
            set { SetValue(LevelPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelPathProperty =
            DependencyProperty.Register(nameof(LevelPath), typeof(string), typeof(TreeEntityBox));
        
        protected override string? GetDisplay(DataTable entities)
        {
            var levelPath = string.IsNullOrWhiteSpace(LevelPath) ? ServiceParameter + "_Code" : LevelPath;
            return EntityDataHelper.GetTreeDisplay(string.IsNullOrWhiteSpace(ValuePath) ? ServiceParameter + "_Id" : ValuePath,
                levelPath,
                string.IsNullOrWhiteSpace(DisplayPath) ? ServiceParameter + "_Name" : DisplayPath,
                LevelLength, entities, Value);
        }

        protected override object? SelectData()
        {
            var dialog = new EntitySelectDialog()
            {
                Title = GlobalCommon.GetMessage("EntitySelectDialog.Title", EntityName),
                DataContext = new TreeEntitySelectViewModel
                {
                    IsRequired = IsRequired,
                    ServiceParameter = ServiceParameter,
                    Conditions = GetSourceRow()?.ToDictionary(),
                    LevelLength = LevelLength,
                    LevelPath = LevelPath,
                },
            };

            var viewModel = (TreeEntitySelectViewModel)dialog.DataContext;
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
