using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class TeacherToSubject
    {
        public int Id { get; set; }

        public string TeacherId { get; set; }

        public int SubjectId { get; set; }
    }
}