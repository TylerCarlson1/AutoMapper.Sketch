using System;

namespace AutoMapper.Sketch.DataAccessLayer
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? DayOfBirth { get; set; }
        public string JobPosition { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string Town { get; set; }
    }
}