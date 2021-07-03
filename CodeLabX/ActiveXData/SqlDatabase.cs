using CodeLabX.DependencyInjection;
using CodeLabX.EntityFramework.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CodeLabX.ActiveXData
{
    public interface ISqlDatabase
    {
        Task<IEnumerable<T>> EntitySet<T>(string query, Func<SqlDataReader, T> dataMapper);
        void ExecuteStordProcNonQuery(string stordProc, List<SqlParameter> sqlParameters = null);
        Task<IEnumerable<T>> ExecuteStordProcQuery<T>(string stordProc, List<SqlParameter> sqlParameters = null);
    }

    public class SqlDatabase : ISqlDatabase
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public SqlDatabase(IRepository repository, ILogger logger)
        {
            _logger = logger;
            _connectionString = repository.DataContext().Database.GetDbConnection().ConnectionString;
        }

        public async Task<IEnumerable<T>> EntitySet<T>(string query, Func<SqlDataReader, T> dataMapper)
        {
            var entitySet = new List<T>();
            try
            {
                SqlDataReader reader;
                _logger.LogInformation("Connecting...");
                using (var con = new SqlConnection(_connectionString))
                {
                    _logger.LogInformation($"Connection string: {_connectionString}");
                    using (var command = new SqlCommand(query, con))
                    {
                        con.Open();
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var property = new JObject();
                            foreach (var prop in typeof(T).GetProperties())
                                property[prop.Name] = reader[prop.Name].ToString();

                            entitySet.Add(property.ToObject<T>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Log: {ex.Message}");
                throw new Exception(ex.Message);
            }

            return await Task.FromResult(entitySet);
        }

        public void ExecuteStordProcNonQuery(string stordProc, List<SqlParameter> sqlParameters = null)
        {
            _logger.LogInformation("Connecting...");
            using var con = new SqlConnection(_connectionString);
            _logger.LogInformation($"Connection string: {_connectionString}");

            using var command = new SqlCommand(stordProc, con);
            command.CommandType = CommandType.StoredProcedure;

            if (sqlParameters is not null)
                command.Parameters.AddRange(sqlParameters.ToArray());

            con.Open();
            command.ExecuteNonQuery();
        }

        public async Task<IEnumerable<T>> ExecuteStordProcQuery<T>(string stordProc, List<SqlParameter> sqlParameters = null)
        {
            _logger.LogInformation("Connecting...");
            using (var con = new SqlConnection(_connectionString))
            {
                _logger.LogInformation($"Connection string: {_connectionString}");
                using (var command = new SqlCommand(stordProc, con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (sqlParameters is not null)
                        command.Parameters.AddRange(sqlParameters.ToArray());

                    con.Open();
                    var reader = command.ExecuteReader();

                    var entitySet = new List<T>();
                    while (reader.Read())
                    {
                        var property = new JObject();
                        foreach (var prop in typeof(T).GetProperties())
                            property[prop.Name] = reader[prop.Name].ToString();

                        entitySet.Add(property.ToObject<T>());
                    }

                    return await Task.FromResult(entitySet);
                }
            }
        }
    }
}
