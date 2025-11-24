// <copyright file="ComplaintsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.Services
{
    using LitShare.BLL.DTOs;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

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
                            Id = c.id,
                            Text = c.text,
                            BookTitle = p.title,
                            UserName = u.name,
                            Date = c.date,
                        };

            return query.ToList();
        }

        public Complaints? GetComplaintWithDetails(int complaintId)
        {
            return this.context.complaints
                .Include(c => c.Post)
                .FirstOrDefault(c => c.id == complaintId);
        }

        public void DeleteComplaint(int complaintId)
        {
            var complaint = this.context.complaints.Find(complaintId);
            if (complaint != null)
            {
                this.context.complaints.Remove(complaint);
                this.context.SaveChanges();
            }
        }

        public void AddComplaint(string reasonText, int postId, int complainantId)
        {
            var newComplaint = new Complaints
            {
                text = reasonText,
                post_id = postId,
                complainant_id = complainantId,
                date = DateTime.UtcNow,
            };

            this.context.complaints.Add(newComplaint);
            this.context.SaveChanges();
        }

        public void ApproveComplaint(int complaintId)
        {
            var complaint = this.context.complaints
                .Include(c => c.Post)
                    .ThenInclude(p => p.BookGenres)
                .FirstOrDefault(c => c.id == complaintId);

            if (complaint == null)
            {
                throw new Exception("Скаргу не знайдено.");
            }

            if (complaint.Post != null && complaint.Post.BookGenres.Any())
            {
                this.context.bookGenres.RemoveRange(complaint.Post.BookGenres);
            }

            if (complaint.Post != null)
            {
                this.context.posts.Remove(complaint.Post);
            }

            this.context.complaints.Remove(complaint);

            this.context.SaveChanges();
        }
    }
}