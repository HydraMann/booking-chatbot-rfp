using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingBotRFT.Dialogs.Data
{
    public class UserProfile
    {
        private string hotel;

        private string date;

        private string name;

        private string email;

        public string Id;

        public string Hotel
        {
            get { return hotel; }
            set { hotel = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public UserProfile(string _hotel, string _date, string _name, string _email)
        {
            Hotel = _hotel;
            Date = _date;
            Name = _name;
            Email = _email;
        }

        public UserProfile()
        {
            
        }
    }
}
