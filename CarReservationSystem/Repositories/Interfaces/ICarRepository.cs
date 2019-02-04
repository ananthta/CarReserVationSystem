using System.Data.SqlClient;
using System.Collections.Generic;
using CarReservationSystem.Models;

namespace CarReservationSystem.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Car Get(int carId);
        List<Car> GetAll();
        Car Get(string model);
        void Reserve(Car car);
        void Reserve(Car car, SqlConnection connection, SqlTransaction transaction);
    }
}
