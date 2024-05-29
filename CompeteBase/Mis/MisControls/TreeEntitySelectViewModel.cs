using System;
using System.Collections.Generic;
using System.Windows;

namespace Compete.Mis.MisControls
{
    internal class TreeEntitySelectViewModel : AbstractEntitySelectViewModel
    {
        private string code = string.Empty;

        private int[]? levelLengths;

        private string? _levelLength;

        public string? LevelLength
        {
            get => _levelLength;
            set
            {
                _levelLength = value;
                if (!string.IsNullOrWhiteSpace(_levelLength))
                {
                    var lengths = _levelLength.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var lengthList = new List<int>();
                    foreach (var length in lengths)
                        lengthList.Add(Convert.ToInt32(length));
                    levelLengths = lengthList.ToArray();
                }
            }
        }

        public string? LevelPath { get; set; }

        protected override void Querying()
        {
            base.Querying();

            if (!Conditions!.TryAdd("code", code ?? string.Empty))
                Conditions!["code"] = code ?? string.Empty;

            var len = code.Length;
            var sum = 0;
            for (var i = 0; i < levelLengths!.Length && sum <= len; i++)
                sum += levelLengths[i];

            if (!Conditions!.TryAdd("code_Length", sum))
                Conditions!["code_Length"] = sum;
        }

        protected override void DoOk(FrameworkElement sender)
        {
            //SelectedItem
            if (true.Equals(Utils.DataHelper.GetValue(SelectedItem!, "Is_Leaf")))
                base.DoOk(sender);
            else
            {
                code = (Utils.DataHelper.GetValue(SelectedItem!, LevelPath!) ?? string.Empty).ToString()!;
                QueryData();
            }
        }

        protected override bool CanOk() => base.CanOk() && null != LevelPath && null != LevelLength;
    }
}
