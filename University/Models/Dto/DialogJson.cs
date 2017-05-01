using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class DialogJson
    {
        public DialogJson()
        {

        }

        public DialogJson(DialogDto dialog)
        {
            Dialog = dialog;
        }

        public DialogDto Dialog { get; set; }

        public bool IsNewDialog { get; set; }
    }
}