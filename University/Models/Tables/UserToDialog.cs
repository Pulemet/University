using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class UserToDialog
    {
        public int Id { get; set; }

        public ApplicationUser User { get; set; }

        public Dialog Dialog { get; set; }
    }
}