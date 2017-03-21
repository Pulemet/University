using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [MaxLength]
        public string Text { get; set; }

        public string SenderId { get; set; }

        public DateTime DateSend { get; set; }

        public int DialogId { get; set; }
    }
}