using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class UserToConversation
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int DialogId { get; set; }
    }
}