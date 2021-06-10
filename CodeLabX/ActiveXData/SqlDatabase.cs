using CodeLabX.DependencyInjection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CodeLabX.ActiveXData
{
    public interface ISqlDatabase
    {
        void ExecuteStordProc(string stordProc, Action<object> callback = null, List<SqlParameter> sqlParameters = null);
        Task<DataTable> GetData(string query);
    }

    public class SqlDatabase : ISqlDatabase
    {
        private readonly ILogger logger;

        public SqlDatabase(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<DataTable> GetData(string query)
        {
            var dataTable = new DataTable();
            try
            {
                SqlDataReader reader;
                logger.LogInformation($"Connection string: {Configuration.ConnectionString}");
                using (var con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();

                    using (var command = new SqlCommand(query, con))
                    {
                        reader = command.ExecuteReader();
                        dataTable.Load(reader);

                        while (reader.Read())
                        {
                            var record = (IDataRecord)reader;
                            logger.LogInformation($"Read data: {record["Name"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return await Task.FromResult(dataTable);
        }

        public void ExecuteStordProc(string stordProc, Action<object> callback = null, List<SqlParameter> sqlParameters = null)
        {
            using var con = new SqlConnection(Configuration.ConnectionString);
            con.Open();

            using var command = new SqlCommand(stordProc, con);
            command.CommandType = CommandType.StoredProcedure;

            if (sqlParameters is not null)
                command.Parameters.AddRange(sqlParameters.ToArray());

            command.ExecuteNonQuery();
            con.Close();

            if (callback is not null)
                callback(command.Parameters);
        }
    }
}
