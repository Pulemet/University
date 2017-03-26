using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    //Кафедра
    public class Department
    {
        public int Id { get; set; }

        public string NameFull { get; set; }

        public string NameAbridgment { get; set; }
    }
}