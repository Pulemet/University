using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class AnswerDto
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public string Text { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }
    }
}