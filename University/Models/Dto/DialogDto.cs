using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Tables;

namespace University.Models.Dto
{
    public class DialogDto
    {
        public int Id { get; set; }

        public List<Message> Messages { get; set; }
    }
}