using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class DialogToUsersDto
    {
        public int Id { get; set; }

        public List<ApplicationUser> Members { get; set; }

        public DialogToUsersDto()
        {
            Members = new List<ApplicationUser>();
        }
    }
}