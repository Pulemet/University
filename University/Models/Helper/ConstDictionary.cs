using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Helper
{
    public class ConstDictionary
    {
        public const string ROLE_ADMIN = "admin";
        public const string ROLE_TEACHER = "teacher";
        public const string ROLE_STUDENT = "student";
        public const string NO_IMAGE = "/Files/icons/NoImage.jpg";
        public const string AVATARS_FOLDER = "/Files/Avatars/";
        public const string MATERIALS_FOLDER = "/Files/Materials/";
    }

    public enum TypeLesson
    {
        Lecture,
        Practic
    }

    public enum UserRoles
    {
        teacher,
        student,
        admin
    }
}