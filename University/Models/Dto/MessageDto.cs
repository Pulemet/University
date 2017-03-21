using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class MessageDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string SenderId { get; set; }

        public DateTime DateSend { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }
    }
}