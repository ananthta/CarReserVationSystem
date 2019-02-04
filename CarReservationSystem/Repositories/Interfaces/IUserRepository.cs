using System;
using System.Collections.Generic;
using CarReservationSystem.Models;

namespace CarReservationSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void RegisterNewUser(User user);
        User Get(Tuple<string, string> name, string email);
    }
}
