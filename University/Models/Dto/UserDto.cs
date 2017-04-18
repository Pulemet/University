using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            SurName = user.SurName;
            PatronymicName = user.PatronymicName;
            BirthDate = user.BirthDate;
            Photo = user.Photo;
            Email = user.Email;
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string PatronymicName { get; set; }

        public string Photo { get; set; }

        public DateTime BirthDate { get; set; }

        public string Group { get; set; }

        public string Email { get; set; }
    }
}