using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Question
    {
        public int Id { get; set; }

        public string Topic { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreateDate { get; set; }

        public int SubjectId { get; set; }

        public string TypeQuestion { get; set; }
    }
}