using System;
using NSubstitute;
using NUnit.Framework;
using CarReservationSystem.Models;
using CarReservationSystem.Services;
using NSubstitute.ReturnsExtensions;
using CarReservationSystem.Configurations;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystems.Tests.Services
{
    [TestFixture]
    public class ReservationRunnerTests
    {
        private IReservationsRunner _target;
        private ICarInformationService _carInfoProvider;
        private IUserInformationService _userInfoProvider;
        private IReservationsService _reservationsService;
        private IReservationsRepository _reservationsRepository;

        [SetUp]
        public void SetUp()
        {
            _carInfoProvider        = Substitute.For<ICarInformationService>();
            _userInfoProvider       = Substitute.For<IUserInformationService>();
            _reservationsService    = Substitute.For<IReservationsService>();
            _reservationsRepository = Substitute.For<IReservationsRepository>();

            _target = new ReservationsRunner(_carInfoProvider, _userInfoProvider, _reservationsService, _reservationsRepository);
        }

        [Test]
        public void Run_WithMakeReservationOption_ShouldMakeNewReservation()
        {
            // Arrange.
            var car = new Car{CarId = 1, Model = "Camry", Quantity = 2};
            var user = new User{UserId = 2, Email = "ananth.tatachar@gmail.com", FirstName = "Ananth", LastName = "Tatachar"};
            var reservationOption = new ReservationOptions
            {
                NewReservation = true, CancelReservation = false, CarModel = car.Model, Email = user.Email,
                FromDate = new DateTime(2018, 01, 03), ToDate = new DateTime(2018, 01, 05),
                UserName = $"{user.FirstName} {user.LastName}"
            };

            _carInfoProvider.GetCar(reservationOption.CarModel).ReturnsForAnyArgs(car);
            _userInfoProvider.GetUser(new Tuple<string, string>(user.FirstName, user.LastName), user.Email).ReturnsForAnyArgs(user);
            _reservationsRepository.GetReservation(user.UserId, car.CarId, reservationOption.FromDate,
                reservationOption.ToDate).ReturnsNullForAnyArgs();

            _reservationsService.TryReserve(reservationOption, car, user).Returns(1);

           // Act.
           var result = _target.Run(reservationOption);

           // Assert.
           Assert.That(result == 1);
        }

        [Test]
        public void Run_WithMakeReservationWithExistingOverlappingReservation_ShouldMakeNewReservation()
        {
            // Arrange.
            var car = new Car { CarId = 1, Model = "Camry", Quantity = 2 };
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

            var existingReservation = new Reservation
            {
                Id = 2,
                CarId = 1,
                UserId = 2,
                Status = Constants.ReservationStatus.Reserved,
                FromDate = new DateTime(2018, 01, 02),
                ToDate = new DateTime(2018, 01, 04)
            };
            _carInfoProvider.GetCar(reservationOption.CarModel).ReturnsForAnyArgs(car);
            _userInfoProvider.GetUser(new Tuple<string, string>(user.FirstName, user.LastName), user.Email).ReturnsForAnyArgs(user);
            _reservationsRepository.GetReservation(user.UserId, car.CarId, reservationOption.FromDate,
                reservationOption.ToDate).ReturnsForAnyArgs(existingReservation);

            _reservationsService.TryReserve(reservationOption, car, user).Returns(1);

            // Act + Assert.
            var ex = Assert.Throws<ArgumentException>(() => _target.Run(reservationOption));
            Assert.That(ex.ToString().Contains("Reservation already exists"));
        }

        [Test]
        public void Run_WithCancelReservationOption_ShouldCancelReservation()
        {
            // Arrange.
            var car = new Car { CarId = 1, Model = "Camry", Quantity = 2 };
            var user = new User { UserId = 2, Email = "ananth.tatachar@gmail.com", FirstName = "Ananth", LastName = "Tatachar" };
            var reservationOption = new ReservationOptions
            {
                NewReservation = false,
                CancelReservation = true,
                CarModel = car.Model,
                Email = user.Email,
                FromDate = new DateTime(2018, 01, 01),
                ToDate = new DateTime(2018, 01, 05),
                UserName = $"{user.FirstName} {user.LastName}"
            };

            var existingReservation = new Reservation
            {
                Id = 2,
                CarId = 1,
                UserId = 2,
                Status = Constants.ReservationStatus.Reserved,
                FromDate = new DateTime(2018, 01, 02),
                ToDate = new DateTime(2018, 01, 04)
            };

            _carInfoProvider.GetCar(reservationOption.CarModel).ReturnsForAnyArgs(car);
            _userInfoProvider.GetUser(new Tuple<string, string>(user.FirstName, user.LastName), user.Email).ReturnsForAnyArgs(user);
            _reservationsRepository.GetReservation(user.UserId, car.CarId, reservationOption.FromDate,
                reservationOption.ToDate).ReturnsForAnyArgs(existingReservation);

            _reservationsService.TryCancel(existingReservation, car, user).Returns(1);

            // Act.
            var result = _target.Run(reservationOption);

            // Assert.
            Assert.That(result == 1);
        }

        [Test]
        public void Run_WithCancelReservationOptionAndNoExistingReservation_ShouldCancelReservation()
        {
            // Arrange.
            var car = new Car { CarId = 1, Model = "Camry", Quantity = 2 };
            var user = new User { UserId = 2, Email = "ananth.tatachar@gmail.com", FirstName = "Ananth", LastName = "Tatachar" };
            var reservationOption = new ReservationOptions
            {
                NewReservation = false,
                CancelReservation = true,
                CarModel = car.Model,
                Email = user.Email,
                FromDate = new DateTime(2018, 01, 03),
                ToDate = new DateTime(2018, 01, 05),
                UserName = $"{user.FirstName} {user.LastName}"
            };

            _carInfoProvider.GetCar(reservationOption.CarModel).ReturnsForAnyArgs(car);
            _userInfoProvider.GetUser(new Tuple<string, string>(user.FirstName, user.LastName), user.Email).ReturnsForAnyArgs(user);
            _reservationsRepository.GetReservation(user.UserId, car.CarId, reservationOption.FromDate,
                reservationOption.ToDate).ReturnsNullForAnyArgs();

            _reservationsService.TryCancel(null, car, user).Returns(1);

            // Act + Assert.
            var ex = Assert.Throws<ArgumentException>(() => _target.Run(reservationOption));
            Assert.That(ex.ToString().Contains("Reservation does not exist"));
        }
    }
}
