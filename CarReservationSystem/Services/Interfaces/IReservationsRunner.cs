using CarReservationSystem.Models;

namespace CarReservationSystem.Services.Interfaces
{
    public interface IReservationsRunner
    {
        int Run(ReservationOptions reservationOptions);
    }
}
