using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Answer
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Text { get; set; }

        public int QuestionId { get; set; }
    }
}