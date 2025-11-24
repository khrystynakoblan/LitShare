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
                        join p in _context.posts on c.post_id equals p.id
                        join u in _context.Users on c.complainant_id equals u.id
                        select new ComplaintDto
                        {
                            Id = c.id,
                            Text = c.text,
                            BookTitle = p.title,
                            UserName = u.name,
                            Date = c.date
                        };

            return query.ToList();
        }

        public Complaints? GetComplaintWithDetails(int complaintId)
        {
            return _context.complaints
                .Include(c => c.Post)
                .FirstOrDefault(c => c.id == complaintId);
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
                text = reasonText,
                post_id = postId,
                complainant_id = complainantId,
                date = DateTime.UtcNow
            };

            _context.complaints.Add(newComplaint);
            _context.SaveChanges();
        }

        public void ApproveComplaint(int complaintId)
        {
            var complaint = _context.complaints
                .Include(c => c.Post)
                    .ThenInclude(p => p.BookGenres)
                .FirstOrDefault(c => c.id == complaintId);

            if (complaint == null)
                throw new Exception("Скаргу не знайдено.");

            if (complaint.Post != null && complaint.Post.BookGenres.Any())
                _context.bookGenres.RemoveRange(complaint.Post.BookGenres);

            if (complaint.Post != null)
                _context.posts.Remove(complaint.Post);

            _context.complaints.Remove(complaint);

            _context.SaveChanges();
        }


    }
}