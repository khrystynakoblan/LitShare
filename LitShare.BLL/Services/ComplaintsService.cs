using System;
using System.Collections.Generic;
using System.Linq;
using LitShare.DAL;
using LitShare.BLL.DTOs;
using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore; 

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

        public Complaints GetComplaintWithDetails(int complaintId)
        {
            using (var context = new LitShareDbContext())
            {
                return context.complaints
                    .Include(c => c.Post)
                    .FirstOrDefault(c => c.id == complaintId);
            }
        }

        public void DeleteComplaint(int complaintId)
        {
            using (var context = new LitShareDbContext())
            {
                var complaint = context.complaints.Find(complaintId);
                if (complaint != null)
                {
                    context.complaints.Remove(complaint);
                    context.SaveChanges();
                }
            }
        }

        public void AddComplaint(string reasonText, int postId, int complainantId)
        {
            var newComplaint = new Complaints
            {
                text = reasonText,
                post_id = postId,
                complainant_id = complainantId,
                date = DateTime.Now 
            };

            using (var context = new LitShareDbContext())
            {
                context.complaints.Add(newComplaint);
                context.SaveChanges();
            }
        }
    }
}