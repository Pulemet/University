using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
        public static IdentityRole RoleAdmin = new IdentityRole { Name = UserRoles.Admin };
        public static IdentityRole RoleStudent = new IdentityRole { Name = UserRoles.Student };
        public static IdentityRole RoleTeacher = new IdentityRole { Name = UserRoles.Teacher };
    }

    public enum TypeLesson
    {
        Lecture,
        Practic
    }

    public static class UserRoles
    {
        public const string Teacher = "teacher";
        public const string Student = "student";
        public const string Admin = "admin";
    }

    public enum UserGenders
    {
        Male,
        Female
    }
}