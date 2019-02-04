using System;
using System.Data.SqlClient;
using System.Text;
using NLog;
using CarReservationSystem.Models;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Services
{
    public class CarInfoProvider : ICarInfoProvider
    {
        public CarInfoProvider(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public Car GetCar(string model)
        {
            return _carRepository.Get(model);
        }

        public bool CarExists(string model)
        {
            var car = _carRepository.Get(model);
            return car != null;
        }

        public bool IsCarAvailable(Car car)
        {
            return car.Quantity > 0;
        }

        public StringBuilder GetAllCarModels()
        {
            var cars = _carRepository.GetAll();
            var carModels = new StringBuilder();

            foreach (var car in cars)
            {
                carModels.Append(car.Model).Append(Environment.NewLine);
            }

            return carModels;
        }

        public void ReserveCar(Car car, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                _carRepository.Reserve(car, connection, transaction);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }

        private readonly ICarRepository _carRepository;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
