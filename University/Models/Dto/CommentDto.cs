using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }

        public DateTime DateAdd { get; set; }

        public string Text { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }
    }
}