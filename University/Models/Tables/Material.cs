using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Material
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SubjectId { get; set; }

        public string FileLink { get; set; }

        public string TypeLesson { get; set; }

        public string AuthorId { get; set; }

        public DateTime DateLoad { get; set; }
    }
}