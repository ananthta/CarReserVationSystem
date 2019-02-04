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
    public class UserRepository : IUserRepository
    {
        public UserRepository(IConnectionStringProvider connectionStringProvider)
        {
            _connectionString = connectionStringProvider.GetDbConnectionString();
        }

        public List<User> GetAll()
        {
            const string sql = @" select Id AS UserId, FirstName, LastName, Email from [dbo].[User] ";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<User>(sql).ToList();
            }
        }

        public User Get(Tuple<string, string> name, string email)
        {
            const string sql = @" SELECT Id AS UserId, FirstName, LastName, Email 
                                  FROM [dbo].[User] 
                                  WHERE ( FirstName = @firstName 
                                  AND LastName = @lastName )
                                  OR Email = @email ";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<User>(sql, new {firstName = name.Item1, lastName = name.Item2, email = email}).FirstOrDefault();
            }
        }

        public void RegisterNewUser(User user)
        {
            const string sql = @" INSERT INTO [dbo].[User] VALUES(@firstName, @lastName, @email) ";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    connection.Execute(sql,
                        new {firstName = user.FirstName, lastName = user.LastName, email = user.Email},
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

        private readonly string _connectionString;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
