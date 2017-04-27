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

        public string Name { get; set; }

        public bool IsConversation { get; set; }

        public List<MessageDto> Messages { get; set; }

        public List<ApplicationUser> Members { get; set; }

        public DialogDto()
        {
            Messages = new List<MessageDto>();

            Members = new List<ApplicationUser>();
        }
    }
}