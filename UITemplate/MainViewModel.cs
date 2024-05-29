using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITemplate
{
    internal sealed partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ushort _pageSize;

        [ObservableProperty]
        private ulong _currentPageNo;

        [ObservableProperty]
        private ulong _recordCount;

        [RelayCommand]
        private void Test()
        {
        }

        [RelayCommand]
        private void Test1()
        {
            RecordCount = 100UL;
            //OnPropertyChanged(nameof(RecordCount));

            DataSet data = new DataSet();

            if (0L < (from row in data.Tables["Master"]!.AsEnumerable()
                      where !DBNull.Value.Equals(row["Debit_Amount"]) && 0M == (decimal)row["Debit_Amount"] && 0M == (decimal)row["Debit_Amount"]
                      select row).LongCount())
                ;

            var debitSum = (from row in data.Tables["Master"]!.AsEnumerable()
                            select (decimal)row["Debit_Amount"]).Sum();
            if (debitSum == 0M)
            {
            }

            if (debitSum != (from row in data.Tables["Master"]!.AsEnumerable()
                             select (decimal)row["Credit_Amount"]).Sum())
            {
            }


        }
    }
}
