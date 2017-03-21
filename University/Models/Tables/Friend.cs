using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Friend
    {
        public int Id { get; set; }

        public string UserOneId { get; set; }

        public string UserTwoId { get; set; }
    }
}