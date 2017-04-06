using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class MaterialComment
    {
        public int Id { get; set; }

        public int MaterialId { get; set; }

        public DateTime DateAdd { get; set; }

        public string Text { get; set; }

        public string AuthorId { get; set; }
    }
}