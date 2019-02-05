using System;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using CarReservationSystem.Models;
using CarReservationSystem.Services;
using NSubstitute.ReturnsExtensions;
using CarReservationSystem.Services.Interfaces;

namespace CarReservationSystems.Tests.Services
{
    [TestFixture]
    public class ReservationOptionsValidatorTests
    {
        private IReservationOptionsValidator _target;
        private ICarInformationService _carInfoProvider;
        private IUserInformationService _userInfoProvider;

        [SetUp]
        public void SetUp()
        {
            _carInfoProvider = Substitute.For<ICarInformationService>();
            _userInfoProvider = Substitute.For<IUserInformationService>();

            _target = new ReservationOptionsValidator(_carInfoProvider, _userInfoProvider);
        }

        [Test]
        public void ValidateAction_WithMakeReservationAndCancelReservationSet_ShouldThrowError()
        {
            // Arrange.
            var reservationOption = new ReservationOptions {CancelReservation = true, NewReservation = true};

            // Act + Arrange .
            var ex = Assert.Throws<ArgumentException>(() => _target.ValidateAction(reservationOption));
            Assert.That(ex.ToString().Contains("Cannot cancel and make reservation at the same time"));
        }

        [Test]
        public void ValidateAndRegisterUserIfNotExisting_WithNonExistingUser_ShouldRegisterUser()
        {
            // Arrange .
            const string email = "ananth.tatachar@gmail.com";
            var name = new Tuple<string, string>("Ananth", "Tatachar");

            var userName = $"{name.Item1} {name.Item2}";

            _userInfoProvider.GetFirstAndLastNames(userName)
                .ReturnsForAnyArgs(name);

            _userInfoProvider.GetUser(name, email).ReturnsNullForAnyArgs();

            // Act .
            _target.ValidateAndRegisterUserIfNotExist($"{name.Item1} {name.Item2}", email);

            // Assert .
            _userInfoProvider.Received().RegisterUser(name, email);
        }

        [Test]
        public void ValidateAndRegisterUserIfNotExist_WithExistingUser_ShouldNotRegisterUser()
        {
            // Arrange .
            const string email = "ananth.tatachar@gmail.com";
            var name = new Tuple<string, string>("Ananth", "Tatachar");

            var userName = $"{name.Item1} {name.Item2}";

            _userInfoProvider.GetFirstAndLastNames(userName)
                .ReturnsForAnyArgs(name);

            _userInfoProvider.GetUser(name, email).ReturnsForAnyArgs(new User
                {UserId = 1, FirstName = name.Item1, LastName = name.Item2, Email = email});

            // Act .
            _target.ValidateAndRegisterUserIfNotExist($"{name.Item1} {name.Item2}", email);

            // Assert .
            _userInfoProvider.DidNotReceive().RegisterUser(name, email);
        }

        [Test]
        public void ValidateBookingDates_WithInvalidBookingDates_ShouldThrowException()
        {
            // Arrange .
            var reservationOptions = new ReservationOptions
                {FromDate = new DateTime(2018, 01, 04), ToDate = new DateTime(2018, 01, 02)};

            // Act + Arrange .
            var ex = Assert.Throws<ArgumentException>(() => _target.ValidateBookingDates(reservationOptions));
            Assert.That(ex.ToString().Contains($" {reservationOptions.FromDate} should be less than {reservationOptions.ToDate} "));
        }

        [Test]
        public void ValidateBookingDates_WithValidBookingDates_ShouldNotThrowException()
        {
            // Arrange .
            var reservationOptions = new ReservationOptions
                { FromDate = new DateTime(2018, 01, 01), ToDate = new DateTime(2018, 01, 04) };

            // Act + Arrange .
            _target.ValidateBookingDates(reservationOptions);
        }

        [Test]
        public void Validate_WithInvalidCarModelOption_ShouldThrowException()
        {
            // Arrange .
            var reservationOptions = new ReservationOptions
            {
                UserName = "Ananth Tatachar", NewReservation = true, CancelReservation = false, CarModel = "Unknown",
                Email = "ananth.tatachar@gmail.com", FromDate = new DateTime(2018, 01, 02),
                ToDate = new DateTime(2018, 01, 04)
            };

            _carInfoProvider.CarExists(reservationOptions.CarModel).ReturnsForAnyArgs(false);
            _carInfoProvider.GetAllCarModels().ReturnsForAnyArgs(new StringBuilder("Benz"));

            // Act + Arrange .
            var ex = Assert.Throws<ArgumentException>(() => _target.Validate(reservationOptions));
            Assert.That(ex.ToString().Contains("Invalid car model"));
        }
    }
}
