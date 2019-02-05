using System;
using CarReservationSystem.Models;

namespace CarReservationSystem.Services.Interfaces
{
    public interface IUserInformationService
    {
        Tuple<string, string> GetFirstAndLastNames(string userName);
        User GetUser(Tuple<string, string> userName, string email);
        void RegisterUser(Tuple<string, string> userName, string email);
    }
}
