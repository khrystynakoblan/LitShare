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
        private readonly LitShareDbContext _context;

       
        public ComplaintsService(LitShareDbContext? context = null)
        {
            _context = context ?? new LitShareDbContext();
        }

        public List<ComplaintDto> GetAllComplaints()
        {
            var query = from c in _context.complaints
                        join p in _context.posts on c.PostId equals p.Id
                        join u in _context.Users on c.ComplainantId equals u.Id
                        select new ComplaintDto
                        {
                            Text = c.Text,
                            BookTitle = p.Title,
                            UserName = u.Name,
                            Date = c.Date
                        };

            return query.ToList();
        }

        public Complaints? GetComplaintWithDetails(int complaintId)
        {
            return _context.complaints
                .Include(c => c.Post)
                .FirstOrDefault(c => c.Id == complaintId);
        }

        public void DeleteComplaint(int complaintId)
        {
            var complaint = _context.complaints.Find(complaintId);
            if (complaint != null)
            {
                _context.complaints.Remove(complaint);
                _context.SaveChanges();
            }
        }

        public void AddComplaint(string reasonText, int postId, int complainantId)
        {
            var newComplaint = new Complaints
            {
                Text = reasonText,
                PostId = postId,
                ComplainantId = complainantId,
                Date = DateTime.UtcNow
            };

            _context.complaints.Add(newComplaint);
            _context.SaveChanges();
        }
    }
}