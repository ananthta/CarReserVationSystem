using System;
using System.Data.SqlClient;
using CarReservationSystem.Models;

namespace CarReservationSystem.Repositories.Interfaces
{
    public interface IReservationsRepository
    {
        Reservation GetReservation(int userId, int carId, DateTime fromDate, DateTime toDate);
        void MakeReservation(Reservation reservation, SqlConnection connection, SqlTransaction transaction);
        void UpdateReservation(Reservation reservation, SqlConnection connection, SqlTransaction transaction);
    }
}
