using System;
using System.IO;
using CarReservationSystem.Services.Interfaces;

namespace CarReservationSystem.Services
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private const string LocalDbFileName = "CarReservationDb.mdf";
        private static readonly string ConnectionString = $@"Data Source=(LocalDb)\MSSQLLocalDB;
                                                             AttachDbFilename={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LocalDbFileName)};
                                                             Integrated Security=True;
                                                             Connect Timeout=30;";

        public string GetDbConnectionString()
        {
            return ConnectionString;
        }
    }
}
