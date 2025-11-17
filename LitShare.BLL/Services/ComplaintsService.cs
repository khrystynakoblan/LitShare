
using LitShare.BLL.DTOs;
using LitShare.DAL;

using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace LitShare.BLL.Services
{
    public class ComplaintsService
    {
        private readonly LitShareDbContext context;


        public ComplaintsService(LitShareDbContext? context = null)
        {
            this.context = context ?? new LitShareDbContext();
        }

        public List<ComplaintDto> GetAllComplaints()
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

        public Complaints? GetComplaintWithDetails(int complaintId)
        {
            return context.complaints
                .Include(c => c.Post)
                .FirstOrDefault(c => c.id == complaintId);
        }

        public void DeleteComplaint(int complaintId)
        {
            var complaint = context.complaints.Find(complaintId);
            if (complaint != null)
            {
                context.complaints.Remove(complaint);
                context.SaveChanges();
            }
        }

        public void AddComplaint(string reasonText, int postId, int complainantId)
        {
            var newComplaint = new Complaints
            {
                text = reasonText,
                post_id = postId,
                complainant_id = complainantId,
                date = DateTime.UtcNow
            };

            context.complaints.Add(newComplaint);
            context.SaveChanges();
        }
    }
}