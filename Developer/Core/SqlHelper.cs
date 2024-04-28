using Compete.Extensions;
using System;
using System.Data;
using System.Data.Common;

namespace Compete.Mis.Developer.Core
{
    internal static class SqlHelper
    {
        public static int Execute(Models.DatabaseConnectionSetting setting, Func<DbConnection, DbTransaction, int> func)
        {
            int result = 0;
            DbProviderFactory factory = DbProviderFactories.GetFactory(setting.ProviderName!);
            using (var connection = factory.CreateConnection()!)
            {
                connection.ConnectionString = setting.ConnectionString;
                connection.Open();

                try
                {

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            result = func(connection, transaction);
                            if (result > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();
                        }
                        catch
                        {
                            transaction.Rollback();
                            return -1;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static void AddParameter(IDbCommand command, string name, DbType dbType, object? value = default)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        public static void SetParameter(IDbCommand command, string name, object? value = default) => (command.Parameters[name] as IDataParameter)!.Value = value;

        public static DataSet ExecuteDataSet(Models.DatabaseConnectionSetting setting, Action<DbCommand> action)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(setting.ProviderName!);
            using (var connection = factory.CreateConnection()!)
            {
                connection.ConnectionString = setting.ConnectionString;
                connection.Open();

                try
                {
                    var command = connection.CreateCommand();
                    action(command);

                    var adapter = factory.CreateDataAdapter()!;
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
