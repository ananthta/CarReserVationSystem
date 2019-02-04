using System;
using CommandLine;

namespace CarReservationSystem.Models
{
    public class ReservationOptions 
    {
        [Option('u', "userName", Required = true, HelpText = "FullName of the user making reservation.")]
        public string UserName { get; set; }

        [Option('e', "email", Required = true, HelpText = "Email of the user making registration")]
        public string Email { get; set; }

        [Option('f', "fromDate", Required = true, HelpText = "Booking from date.")]
        public DateTime FromDate { get; set; }

        [Option('t', "toDate", Required = true, HelpText = "Booking to date.")]
        public DateTime ToDate { get; set; }

        [Option('c', "carModel", Required = true, HelpText = "CarModel requested for booking.")]
        public string CarModel { get; set; }

        [Option('n', "newReservation", Required = false, HelpText = "Make new reservation.")]
        public bool NewReservation { get; set; }

        [Option('x', "cancelReservation", Required = false, HelpText = "Cancel reservation.")]
        public bool CancelReservation { get; set; }
    }
}
