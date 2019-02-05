using System;
using NSubstitute;
using NUnit.Framework;
using CarReservationSystem.Models;
using CarReservationSystem.Services;
using CarReservationSystem.Configurations;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystems.Tests.Services
{
    public class ReservationServiceTests
    {
        private IReservationsService _target;
        private ICarInformationService _carInfoProvider;
        private IReservationsRepository _reservationsRepository;
        private IConnectionStringProvider _connectionStringProvider;

        [SetUp]
        public void SetUp()
        {
            _connectionStringProvider = new ConnectionStringProvider();
            _carInfoProvider = Substitute.For<ICarInformationService>();
            _reservationsRepository = Substitute.For<IReservationsRepository>();

            _target = new ReservationsService(_carInfoProvider, _reservationsRepository, _connectionStringProvider);
        }

        [Test]
        public void TryReserve_ValidArguments_ShouldBeReserved()
        {
            // Arrange .
            const int carQuantity = 2;
            var car = new Car { CarId = 1, Model = "Camry", Quantity = carQuantity };
            var user = new User { UserId = 2, Email = "ananth.tatachar@gmail.com", FirstName = "Ananth", LastName = "Tatachar" };
            var reservationOption = new ReservationOptions
            {
                NewReservation = true,
                CancelReservation = false,
                CarModel = car.Model,
                Email = user.Email,
                FromDate = new DateTime(2018, 01, 01),
                ToDate = new DateTime(2018, 01, 05),
                UserName = $"{user.FirstName} {user.LastName}"
            };

            // Act .
            var result = _target.TryReserve(reservationOption, car, user);

            // Assert .
            Assert.That(result == 1);
            Assert.That(car.Quantity == carQuantity-1);
        }

        [Test]
        public void TryReserve_WithZeroCarQty_ShouldNotBeReserved()
        {
            // Arrange .
            const int carQuantity = 0;
            var car = new Car { CarId = 1, Model = "Camry", Quantity = carQuantity };
            var user = new User { UserId = 2, Email = "ananth.tatachar@gmail.com", FirstName = "Ananth", LastName = "Tatachar" };
            var reservationOption = new ReservationOptions
            {
                NewReservation = true,
                CancelReservation = false,
                CarModel = car.Model,
                Email = user.Email,
                FromDate = new DateTime(2018, 01, 01),
                ToDate = new DateTime(2018, 01, 05),
                UserName = $"{user.FirstName} {user.LastName}"
            };

            // Act .
            var result = _target.TryReserve(reservationOption, car, user);

            // Assert .
            Assert.That(result == 0);
        }

        [Test]
        public void TryCancel_WithZeroCarQty_ShouldBeCancelled()
        {
            // Arrange .
            const int carQuantity = 0;
            var car = new Car { CarId = 1, Model = "Camry", Quantity = carQuantity };
            var user = new User { UserId = 2, Email = "ananth.tatachar@gmail.com", FirstName = "Ananth", LastName = "Tatachar" };
            var existingReservation = new Reservation
            {
                Id = 3, CarId = car.CarId, UserId = user.UserId, Status = Constants.ReservationStatus.Reserved,
                FromDate = new DateTime(2018, 01, 02), ToDate = new DateTime(2018, 01, 04)
            };

            // Act .
            var result = _target.TryCancel(existingReservation, car, user);

            // Assert .
            Assert.That(result == 1);
        }
    }
}
