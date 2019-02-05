using NLog;
using System;
using System.Data.SqlClient;
using CarReservationSystem.Models;
using CarReservationSystem.Configurations;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Services
{
    public class ReservationsService : IReservationsService
    {
        public ReservationsService
            (
                ICarInformationService carInfoProvider, 
                IReservationsRepository reservationsRepository,
                IConnectionStringProvider connectionStringProvider
            )
        {
            _carInfoProvider        = carInfoProvider;
            _reservationsRepository = reservationsRepository;
            _connectionString       = connectionStringProvider.GetDbConnectionString();
        }

        public int TryReserve(ReservationOptions reservationOptions, Car car, User user)
        {
            var countOfPreviousReservations =
                _reservationsRepository.GetCountOfPreviousReservation(car.CarId, reservationOptions.FromDate);

            if (car.Quantity <= 0 && (car.Quantity + countOfPreviousReservations) <= 0)
            {
                Logger.Error("Unable to make reservation as no cars of this model are available.");
                return 0;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    car.Quantity = car.Quantity - 1;
                    _carInfoProvider.ReserveCar(car, connection, transaction);
                    _reservationsRepository.MakeReservation(
                        new Reservation
                        {
                            CarId = car.CarId,
                            UserId = user.UserId,
                            Status = Constants.ReservationStatus.Reserved,
                            FromDate = reservationOptions.FromDate,
                            ToDate = reservationOptions.ToDate
                        }, connection, transaction);
                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Logger.Error(e.ToString());
                    throw;
                }
            }
        }


        public int TryCancel(Reservation existingReservation, Car car, User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    car.Quantity = car.Quantity + 1;
                    _carInfoProvider.ReserveCar(car, connection, transaction);
                    existingReservation.Status = Constants.ReservationStatus.Cancelled;
                    _reservationsRepository.UpdateReservation(existingReservation, connection, transaction);
                    transaction.Commit();
                    return 1;
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
        private readonly ICarInformationService _carInfoProvider;
        private readonly IReservationsRepository _reservationsRepository;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
