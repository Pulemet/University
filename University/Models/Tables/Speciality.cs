using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Speciality
    {
        public int Id { get; set; }

        public string NameFull { get; set; }

        public string NameAbridgment { get; set; }

        public Department Department { get; set; }

        public Speciality()
        {
            Department = new Department();
        }
    }
}