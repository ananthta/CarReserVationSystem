using NLog;
using System;
using Dapper;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using CarReservationSystem.Models;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Repositories
{
    public class CarRepository : ICarRepository
    {
        public CarRepository(IConnectionStringProvider connectionStringProvider)
        {
            _connectionString = connectionStringProvider.GetDbConnectionString();
        }

        public Car Get(string model)
        {
            const string sql = @" select Id AS CarId, Model, Quantity 
                                         FROM [dbo].[Car] 
                                  WHERE MODEL LIKE @model ";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Car>(sql, param: new {model}).FirstOrDefault();
            }
        }

        public Car Get(int carId)
        {
            const string sql = @" select Id AS CarId, Model, Quantity 
                                         FROM [dbo].[Car] 
                                  WHERE Id = @id ";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Car>(sql, param: new { id = carId }).FirstOrDefault();
            }
        }

        public List<Car> GetAll()
        {
            const string sql = @" select Id AS CarId, Model, Quantity 
                                         FROM [dbo].[Car] ";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Car>(sql).ToList();
            }
        }

        public void Reserve(Car car)
        {
            const string sql = @" Update [dbo].[Car]
                                  SET Quantity = @quantity
                                  WHERE Id = @id ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    connection.Execute(sql,
                        new { quantity = car.Quantity, id = car.CarId },
                        transaction: transaction);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Logger.Error(e.ToString());
                    throw;
                }
            }
        }

        public void Reserve(Car car, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @" Update [dbo].[Car]
                                  SET Quantity = @quantity
                                  WHERE Id = @id ";
            try
            {
                connection.Execute(sql,
                    new {quantity = car.Quantity, id = car.CarId},
                    transaction: transaction);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }

        private readonly string _connectionString;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
