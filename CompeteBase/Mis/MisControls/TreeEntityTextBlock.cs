using System.Data;
using System.Windows;

namespace Compete.Mis.MisControls
{
    public class TreeEntityTextBlock : AbstractEntityTextBlock
    {
        public string? LevelLength
        {
            get { return (string?)GetValue(LevelLengthProperty); }
            set { SetValue(LevelLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelLengthProperty =
            DependencyProperty.Register(nameof(LevelLength), typeof(string), typeof(TreeEntityTextBlock));

        public string? LevelPath
        {
            get { return (string?)GetValue(LevelPathProperty); }
            set { SetValue(LevelPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelPathProperty =
            DependencyProperty.Register(nameof(LevelPath), typeof(string), typeof(TreeEntityTextBlock));

        protected override string? GetDisplay(DataTable entities)
        {
            var levelPath = string.IsNullOrWhiteSpace(LevelPath) ? ServiceParameter + "_Code" : LevelPath;
            return EntityDataHelper.GetTreeDisplay(string.IsNullOrWhiteSpace(ValuePath) ? ServiceParameter + "_Id" : ValuePath,
                levelPath,
                string.IsNullOrWhiteSpace(DisplayPath) ? ServiceParameter + "_Name" : DisplayPath,
                LevelLength, entities, Value);
        }
    }
}
