using System.Text;
using System.Data.SqlClient;
using CarReservationSystem.Models;

namespace CarReservationSystem.Services.Interfaces
{
    public interface ICarInfoProvider
    {
        Car GetCar(string model);
        bool CarExists(string model);
        bool IsCarAvailable(Car car);
        StringBuilder GetAllCarModels();
        void ReserveCar(Car car, SqlConnection connection, SqlTransaction transaction);
    }
}
