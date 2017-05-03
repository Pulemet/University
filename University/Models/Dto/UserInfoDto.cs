using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Helper;

namespace University.Models.Dto
{
    public class UserInfoDto
    {
        public UserInfoDto() { }

        public UserInfoDto(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            SurName = user.SurName;
            Photo = user.Photo == "" ? ConstDictionary.NO_IMAGE : user.Photo;
            Email = user.Email;
        }
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string Photo { get; set; }

        public string Email { get; set; }

        public string UserRole { get; set; }

        public bool IsFriend { get; set; }
    }
}