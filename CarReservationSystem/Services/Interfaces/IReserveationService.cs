using CarReservationSystem.Models;

namespace CarReservationSystem.Services.Interfaces
{
    public interface IReservationsService
    {
        int TryCancel(Reservation existingReservation, Car car, User user);
        int TryReserve(ReservationOptions reservationOptions, Car car, User user);
    }
}
