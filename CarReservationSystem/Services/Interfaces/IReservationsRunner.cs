using CarReservationSystem.Models;

namespace CarReservationSystem.Services.Interfaces
{
    public interface IReservationsRunner
    {
        void Run(ReservationOptions reservationOptions);
    }
}
