using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitShare.BLL.DTOs
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
    }
}