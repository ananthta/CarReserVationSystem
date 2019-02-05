using NLog;
using System;
using CarReservationSystem.Models;
using CarReservationSystem.Services.Interfaces;

namespace CarReservationSystem.Services
{
    public class ReservationOptionsValidator : IReservationOptionsValidator
    {
        public ReservationOptionsValidator
            (
                ICarInformationService carInfoProvider,
                IUserInformationService userInfoProvider
            )
        {
            _carInfoProvider    = carInfoProvider;
            _userInfoProvider   = userInfoProvider;
        }

        public void Validate(ReservationOptions reservationOptions)
        {
            ValidateAction(reservationOptions);
            ValidateBookingDates(reservationOptions);
            ValidateAndRegisterUserIfNotExist(reservationOptions.UserName, reservationOptions.Email);

            if (_carInfoProvider.CarExists(reservationOptions.CarModel)) return;
            var errorMessage =
                $" Invalid car model: {reservationOptions.CarModel}, available car models are: . {Environment.NewLine} {_carInfoProvider.GetAllCarModels()}";
            Logger.Error(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        public void ValidateAction(ReservationOptions reservationOptions)
        {
            if (!reservationOptions.NewReservation || !reservationOptions.CancelReservation) return;
            const string errorMessage = "Cannot cancel and make reservation at the same time";
            Logger.Error(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        public void ValidateAndRegisterUserIfNotExist(string userName, string email)
        {
            var name = _userInfoProvider.GetFirstAndLastNames(userName);

            if (string.IsNullOrEmpty(email))
            {
                var errorMessage = $" Email: {email} is null or empty.";
                Logger.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var user = _userInfoProvider.GetUser(name, email);

            if(user == null)
                _userInfoProvider.RegisterUser(name, email);
        }

        public void ValidateBookingDates(ReservationOptions reservationOptions)
        {
            if (reservationOptions.ToDate > reservationOptions.FromDate) return;
            {
                var errorMessage = $" {reservationOptions.FromDate} should be less than {reservationOptions.ToDate} ";
                Logger.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }
        }

        private readonly ICarInformationService _carInfoProvider;
        private readonly IUserInformationService _userInfoProvider;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
