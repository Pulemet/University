using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class SubjectsToSemesterInGroup
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }

        public int StudentGroupId { get; set; }

        public int SubjectId { get; set; }
    }
}