using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Dto
{
    public class MaterialInfoForAutor
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public string Name { get; set; }

        public string SubjectName { get; set; }
    }
}