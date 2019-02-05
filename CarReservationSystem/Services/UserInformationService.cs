using NLog;
using System;
using CarReservationSystem.Models;
using CarReservationSystem.Services.Interfaces;
using CarReservationSystem.Repositories.Interfaces;

namespace CarReservationSystem.Services
{
    public class UserInformationService : IUserInformationService
    {
        public UserInformationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Tuple<string, string> GetFirstAndLastNames(string userName)
        {
            var name = userName.Split(' ');

            if (name == null || name.Length != 2)
            {
                var errorMessage = $" UserName: {userName} should be FirstName LastName.";
                Logger.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            if (!string.IsNullOrEmpty(name[0]) && !string.IsNullOrEmpty(name[1]))
                return new Tuple<string, string>(name[0], name[1]);
            {
                var errorMessage = $" FirstName: {name[0]} or LastName: {name[1]} is null or empty.";
                Logger.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }
        }

        public void RegisterUser(Tuple<string, string> userName, string email)
        {
            _userRepository.RegisterNewUser(new User{FirstName = userName.Item1, LastName = userName.Item2, Email = email});
        }

        public User GetUser(Tuple<string, string> userName, string email)
        {
            return _userRepository.Get(userName, email);
        }

        private readonly IUserRepository _userRepository;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    }
}
