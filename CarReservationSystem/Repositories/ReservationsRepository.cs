using NLog;
using System;
using Dapper;
using System.Linq;
using System.Data.SqlClient;
using CarReservationSystem.Models;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Repositories
{
    public class ReservationsRepository : IReservationsRepository
    {
        public ReservationsRepository(IConnectionStringProvider connectionStringProvider)
        {
            _connectionString = connectionStringProvider.GetDbConnectionString();
        }

        public Reservation GetReservation(int userId, int carId, DateTime fromDate, DateTime toDate)
        {
            const string sql = @" select Id, UserId, CarId,
                                         Status, FromDate, ToDate
                                         FROM [dbo].[Reservations] 
                                  WHERE UserId = @userId AND CarId = @carId
                                  AND FromDate >= @fromDate AND ToDate <= @toDate ";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Reservation>(sql, new {userId, carId, fromDate, toDate}).FirstOrDefault();
            }
        }

        public void MakeReservation(Reservation reservation, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @" INSERT INTO [dbo].[Reservations] 
                                      VALUES(@userId, @carId, @status, @fromDate, @toDate) ";
            try
            {
                connection.Execute(sql,
                    new
                    {
                        userId = reservation.UserId, carId = reservation.CarId, status = reservation.Status,
                        fromDate = reservation.FromDate, toDate = reservation.ToDate
                    }, transaction);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public void UpdateReservation(Reservation reservation, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @" UPDATE [dbo].[Reservations] 
                                  SET STATUS = @status
                                  WHERE Id = @reservationId ";

            try
            {
                connection.Execute(sql, new {status = reservation.Status, reservationId = reservation.Id}, transaction);
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
