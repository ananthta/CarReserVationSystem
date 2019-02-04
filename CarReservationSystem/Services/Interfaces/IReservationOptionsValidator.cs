using CarReservationSystem.Models;

namespace CarReservationSystem.Services.Interfaces
{
    public interface IReservationOptionsValidator
    {
        void Validate(ReservationOptions reservationOptions);
        void ValidateAction(ReservationOptions reservationOptions);
        void ValidateBookingDates(ReservationOptions reservationOptions);
        void ValidateAndRegisterUserIfNotExist(string userName, string email);
    }
}
