﻿using NLog;
using System;
using CarReservationSystem.Models;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Services
{
    public class ReservationsRunner : IReservationsRunner
    {
        public ReservationsRunner
            (
                ICarInformationService carInfoProvider, 
                IUserInformationService userInfoProvider,
                IReservationsService reservationsService,
                IReservationsRepository reservationsRepository
            )
        {
            _carInfoProvider        = carInfoProvider;
            _userInfoProvider       = userInfoProvider;
            _reservationsService    = reservationsService;
            _reservationsRepository = reservationsRepository;
        }

        public int Run(ReservationOptions reservationOptions)
        {
            var user = GetUser(reservationOptions);
            var car = _carInfoProvider.GetCar(reservationOptions.CarModel);

            var existingReservation = _reservationsRepository.GetReservation(user.UserId, car.CarId,
                reservationOptions.FromDate, reservationOptions.ToDate);

            if (reservationOptions.NewReservation)
            {
                if (existingReservation != null)
                {
                    const string errorMessage = " Reservation already exists ";
                    Logger.Error(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                var result = _reservationsService.TryReserve(reservationOptions, car, user);
                Logger.Info(result == 1 ? "Reservation made successfully." : "Unable to make reservation.");
                return result;
            }

            if (!reservationOptions.CancelReservation) return 0;
            {
                if (existingReservation == null && reservationOptions.CancelReservation)
                {
                    const string errorMessage = " Reservation does not exist ";
                    Logger.Error(errorMessage);
                    throw new ArgumentException(errorMessage);
                }
                var result = _reservationsService.TryCancel(existingReservation, car, user);
                Logger.Info(result == 1 ? "Reservation cancelled successfully." : "Unable to make reservation.");
                return result;
            }

        }

        private User GetUser(ReservationOptions reservationOptions)
        {
            var userName = _userInfoProvider.GetFirstAndLastNames(reservationOptions.UserName);
            return _userInfoProvider.GetUser(userName, reservationOptions.Email);
        }

        private readonly ICarInformationService _carInfoProvider;
        private readonly IUserInformationService _userInfoProvider;
        private readonly IReservationsService _reservationsService;
        private readonly IReservationsRepository _reservationsRepository;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
