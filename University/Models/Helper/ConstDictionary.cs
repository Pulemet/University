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
        public static IdentityRole RoleAdmin = new IdentityRole { Name = "admin" };
        public static IdentityRole RoleStudent = new IdentityRole { Name = "student" };
        public static IdentityRole RoleTeacher = new IdentityRole { Name = "teacher" };
    }

    public enum TypeLesson
    {
        Lecture,
        Practic
    }

    public enum UserRoles
    {
        Teacher,
        Student,
        Admin
    }

    public enum UserGenders
    {
        Male,
        Female
    }
}