using System;
using System.Collections.Generic;
using System.Linq;
using LitShare.DAL;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services
{
    public class ComplaintsService
    {
        public List<ComplaintDto> GetAllComplaints()
        {
            using (var context = new LitShareDbContext())
            {
                var query = from c in context.complaints
                            join p in context.posts on c.post_id equals p.id
                            join u in context.Users on c.complainant_id equals u.id
                            select new ComplaintDto
                            {
                                Text = c.text,
                                BookTitle = p.title,
                                UserName = u.name,
                                Date = c.date
                            };

                return query.ToList();
            }
        }
    }
}
