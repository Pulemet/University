using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Helper;

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
            Gender = user.Gender == "female" ? UserGenders.Female : UserGenders.Male;
            BirthDate = user.BirthDate;
            Photo = user.Photo == "" ? ConstDictionary.NO_IMAGE : user.Photo;
            Email = user.Email;
            UserInfo = user.UserInfo;
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string PatronymicName { get; set; }

        public string Photo { get; set; }

        public DateTime BirthDate { get; set; }

        public string Group { get; set; }

        public string Speciality { get; set; }

        public string Faculty { get; set; }

        public string Email { get; set; }

        public string UserRole { get; set; }

        public string Department { get; set; }

        public UserGenders Gender { get; set; }

        public string UserInfo { get; set; }
    }
}