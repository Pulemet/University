using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Semester
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public DateTime StarTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}