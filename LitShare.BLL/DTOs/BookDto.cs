﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitShare.BLL.DTOs
{
    public class BookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string DealType { get; set; }
        public string Location { get; set; }
        public string ImagePath { get; set; }
    }
}