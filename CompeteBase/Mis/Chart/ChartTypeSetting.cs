using ScottPlot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Compete.Mis.Chart
{
    public sealed class ChartTypeSetting
    {
        private static readonly IList<Color> colors = new List<Color>
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Yellow,
            Colors.Cyan,
            Colors.Fuchsia,
            Colors.Orange,
            Colors.Purple,
            Colors.Olive,
            Colors.Teal,
            Colors.Navy,
            Colors.Maroon,
            Colors.Lime,
            Colors.Magenta,
            Colors.Brown,
            Colors.Pink,
            Colors.Gray,
            Colors.LightBlue,
            Colors.LightGreen,
            Colors.LightCoral,
        };

        private static byte colorIndex = 0;

        public static Color GetNextColor()
        {
            if (colorIndex >= colors.Count)
                colorIndex = 0;
            return colors[colorIndex++];
        }

        private ChartTypeSetting()
        {
        }

        public string DisplayName { get; set; } = string.Empty;

        public Action<IEnumerable, ChartAxisSetting, PlottableAdder>? AddItem { get; set; }

        public static IDictionary<ChartType, ChartTypeSetting> Settings { get; } = new Dictionary<ChartType, ChartTypeSetting>
        {
            {
                ChartType.Signal,
                new ChartTypeSetting
                {
                    DisplayName = GlobalCommon.GetMessage("Chart.Signal"),    // 信号图
                    AddItem = (data, setting, add) => add.Signal((from rowView in data.Cast<DataRowView>()
                                                                  select Convert.ToDouble(rowView[setting.DataPath[0]])).ToArray(), 1, setting.FillColor == null ? null : new Color(setting.FillColor.Value)),
                }
            },
            {
                ChartType.Bars,
                new ChartTypeSetting
                {
                    DisplayName = GlobalCommon.GetMessage("Chart.Bars"),    // 柱状图
                    AddItem = (data, setting, add) =>
                    {
                        var bar = add.Bars((from rowView in data.Cast<DataRowView>()
                                            select Convert.ToDouble(rowView[setting.DataPath[0]])).ToArray());
                        if (setting.FillColor is not null)
                            bar.Color = setting.FillColor == null ? GetNextColor() : new Color(setting.FillColor.Value);
                     },
                }
            },
            {
                ChartType.Pie,
                new ChartTypeSetting
                {
                    DisplayName = GlobalCommon.GetMessage("Chart.Pie"),     // 饼图
                    AddItem = (data, setting, add) => add.Pie((from rowView in data.Cast<DataRowView>()
                                                               select new PieSlice
                                                               {
                                                                   Value = Convert.ToDouble(rowView[setting.DataPath[0]]),
                                                                   Label = setting.DataPath.Count > 1 ? rowView[setting.DataPath[1]].ToString()! : string.Empty,
                                                                   FillColor = setting.FillColor == null ? GetNextColor() : new Color(setting.FillColor.Value),
                                                               }).ToList()),
                }
            },
            {
                ChartType.Radar,
                new ChartTypeSetting
                {
                    DisplayName = GlobalCommon.GetMessage("Chart.Radar"),    // 雷达图
                    AddItem = (data, setting, add) =>
                    {
                        var radar = add.Radar();
                        foreach(var item in from rowView in data.Cast<DataRowView>()
                                            select new RadarSeries
                                            {
                                                Values = (from name in setting.DataPath
                                                        select Convert.ToDouble(rowView[name])).ToArray(),
                                                FillColor = setting.FillColor == null ? GetNextColor() : new Color(setting.FillColor.Value),
                                                LineColor = setting.LineColor == null ? Colors.Black : new Color(setting.LineColor.Value),
                                            })
                            radar.Series.Add(item);
                    },
                }
            },
            {
                ChartType.Scatter,
                new ChartTypeSetting
                {
                    DisplayName = GlobalCommon.GetMessage("Chart.Scatter"),    // 散布图
                    AddItem = (data, setting, add) =>
                    {
                        var list = (from rowView in data.Cast<DataRowView>()
                                    select new Coordinates
                                    {
                                        X = Convert.ToDouble(rowView[setting.DataPath[0]]),
                                        Y = Convert.ToDouble(rowView[setting.DataPath[1]]),
                                    }).ToList();
                        if (setting.FillColor is null)
                            add.Scatter(list);
                        else
                            add.Scatter(list, new Color(setting.FillColor.Value));
                    },
                }
            },
        };

        public static ChartTypeSetting? DefaultChartTypeSetting { get; } = Settings.Values.First();
    }
}
