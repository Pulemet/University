using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Helper;
using University.Models.Tables;

namespace University.Models.Dto
{
    public class TeacherDto
    {
        public TeacherDto()
        {
            Subjects = new List<Subject>();
            Comments = new List<CommentToTeacherDto>();
        }

        public TeacherDto(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            SurName = user.SurName;
            PatronymicName = user.PatronymicName;
            Email = user.Email;
            Photo = user.Photo == "" ? ConstDictionary.NO_IMAGE : user.Photo;
            Subjects = new List<Subject>();
            Comments = new List<CommentToTeacherDto>();
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string PatronymicName { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        public string Department { get; set; }

        public List<Subject> Subjects { get; set; }

        public List<CommentToTeacherDto> Comments { get; set; } 
    }
}