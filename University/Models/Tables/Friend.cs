using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class Friend
    {
        public int Id { get; set; }

        public ApplicationUser UserOne { get; set; }

        public ApplicationUser UserTwo { get; set; }

        public Friend()
        {
            UserOne = new ApplicationUser();

            UserTwo  = new ApplicationUser();
        }
    }
}