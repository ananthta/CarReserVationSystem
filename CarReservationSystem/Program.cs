using System;
using CommandLine;
using CarReservationSystem.Models;
using CarReservationSystem.Configurations;
using CarReservationSystem.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CarReservationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = DependencyInjectionServiceProvider.Get();
            var validator = serviceProvider.GetService<IReservationOptionsValidator>(); 
            var reservationsRunner = serviceProvider.GetService<IReservationsRunner>();

            Parser.Default.ParseArguments<ReservationOptions>(args).WithParsed(o =>
            {
                validator.Validate(o);
                reservationsRunner.Run(o);
            });

            Console.ReadKey();
        }
    }
}
