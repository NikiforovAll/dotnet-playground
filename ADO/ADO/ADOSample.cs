using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ADO
{
    class ADOSample
    {
        //TBD: consider moving queries to external source and initialize as dictionary from source
        private Dictionary<string, string> _querySource = new Dictionary<string, string>
        {
            [nameof(ExecuteScalar)] = "SELET COUNT(*) FROM Production.Product",

            [nameof(ExecuteReader)] = @"SELECT Prod.ProductID, Prod.Name, Prod.StandardCost, Prod.ListPrice,
                                        CostHistory.StartDate, CostHistory.EndDate, CostHistory.StandardCost
                                        FROM Production.ProductCostHistory AS CostHistory 
                                        INNER JOIN Production.Product AS Prod ON CostHistory.ProductId = Prod.ProductId
                                        WHERE Prod.ProductId = @ProductId",

            [nameof(TransactionSample)] = @"INSERT INTO Sales.CreditCard (CardType, CardNumber, ExpMonth, ExpYear)
                                            VALUES (@CardType, @CardNumber, @ExpMonth, @ExpYear); 
                                            SELECT SCOPE_IDENTITY()"
        };

        private IConfiguration _configuration;

        public ADOSample()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
        }

        public string GetConnectionString()
        {
            return _configuration["Data:DefaultConnection:ConnectionString"];
        }

        /// <summary>
        ///    Example 1
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExecuteScalar()
        {
            _querySource.TryGetValue(nameof(ExecuteScalar), out string query);
            var resultRaw = string.Empty;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                resultRaw = await command.ExecuteScalarAsync() as string;
            }
            return int.Parse(String.IsNullOrWhiteSpace(resultRaw) ? resultRaw : "-1");
        }

        /// <summary>
        ///     Example 2 
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> ExecuteReader(int productId)
        {
            var productInfo = new List<string>();
            _querySource.TryGetValue(nameof(ExecuteReader), out string query);
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(
                    new SqlParameter("ProductionId", SqlDbType.Int)
                    {
                        Value = productId
                    }
                );
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)reader["ProductId"];
                        string name = reader["Name"] as string;
                        DateTime from = (DateTime)reader["StartDate"];
                        DateTime? to = (DateTime?)reader["EndDate"];
                        productInfo.Add($"{id}-{name}-{from: yyyy-MM-dd}--{to: yyyy-MM-dd}");
                    }
                }
            }
            return productInfo;
        }

        private async Task TransactionSample()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                SqlTransaction tx = connection.BeginTransaction();

                try
                {
                    _querySource.TryGetValue(nameof(TransactionSample), out string query);
                    var command = new SqlCommand()
                    {
                        CommandText = query,
                        Connection = connection,
                        Transaction = tx
                    };
                    command.Parameters.AddRange(
                        new SqlParameter[] {
                            new SqlParameter("CardType", SqlDbType.NVarChar, 50){
                                Value = "Test1"
                            },
                            new SqlParameter("CardNumber", SqlDbType.NVarChar, 25){
                                Value = "1111111111111"
                            },
                            new SqlParameter("ExpMonth", SqlDbType.TinyInt){
                                Value = 4
                            },
                            new SqlParameter("ExpYear", SqlDbType.SmallInt)
                            {
                                Value = 2018
                            }
                        }
                    );

                    object id = await command.ExecuteScalarAsync();
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error {ex.Message}, rolling back");
                    tx.Rollback();
                }
            }
        }

    }
}
