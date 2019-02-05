using System;
using CarReservationSystem.Services;
using CarReservationSystem.Repositories;
using CarReservationSystem.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Configurations
{
    public static class DependencyInjectionServiceProvider
    {
        public static IServiceProvider Get()
        {
            if (_serviceProvider != null)
                return _serviceProvider;

            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ICarRepository, CarRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<ICarInformationService, CarInformationService>()
                .AddSingleton<IUserInformationService, UserInformationService>()
                .AddSingleton<IReservationsRunner, ReservationsRunner>()
                .AddSingleton<IReservationsService, ReservationsService>()
                .AddSingleton<IReservationsRepository, ReservationsRepository>()
                .AddSingleton<IConnectionStringProvider, ConnectionStringProvider>()
                .AddSingleton<IReservationOptionsValidator, ReservationOptionsValidator>()
                .BuildServiceProvider();
            return _serviceProvider;
        }

        private static IServiceProvider _serviceProvider;
    }
}
