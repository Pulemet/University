using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Tables;

namespace University.Models.Dto
{
    public class MessageDto
    {
        public MessageDto()
        {
            
        }

        public MessageDto(Message message)
        {
            Id = message.Id;
            Text = message.Text;
            DateSend = string.Format("{0:dd.MM.yyyy} в {0:H.mm}", message.DateSend);
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public string DateSend { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }
    }
}