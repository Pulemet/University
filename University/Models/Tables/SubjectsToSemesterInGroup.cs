using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class SubjectsToSemesterInGroup
    {
        public int Id { get; set; }
        public Semester Semester { get; set; }

        public StudentGroup StudentGroup { get; set; }

        public Subject Subject { get; set; }

        public SubjectsToSemesterInGroup()
        {
            Semester = new Semester();

            StudentGroup = new StudentGroup();

            Subject = new Subject();
        }
    }
}