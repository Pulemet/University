﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Models.Tables
{
    public class StudentGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SpecialityId { get; set; }
    }
}