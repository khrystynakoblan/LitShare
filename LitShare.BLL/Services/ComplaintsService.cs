// <copyright file="ComplaintsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.Services
{
    using LitShare.BLL.DTOs;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides services for managing and processing user complaints against posts.
    /// </summary>
    public class ComplaintsService
    {
        private readonly LitShareDbContext context; // SA1300 Fix: Renamed to _context

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintsService"/> class.
        /// </summary>
        /// <param name="context">The database context (optional, defaults to a new instance if null).</param>
        public ComplaintsService(LitShareDbContext? context = null)
        {
            this.context = context ?? new LitShareDbContext();
        }

        /// <summary>
        /// Retrieves a list of all complaints with necessary details for display.
        /// </summary>
        /// <returns>A list of <see cref="ComplaintDto"/> objects.</returns>
        public List<ComplaintDto> GetAllComplaints()
        {
            var query = from c in this.context.Complaints
                        join p in this.context.Posts on c.PostId equals p.Id // FKs use PascalCase
                        join u in this.context.Users on c.ComplainantId equals u.Id // FKs use PascalCase
                        select new ComplaintDto
                        {
                            Id = c.Id,
                            Text = c.Text,
                            BookTitle = p.Title, // Uses PascalCase C# property
                            UserName = u.Name, // Uses PascalCase C# property
                            Date = c.Date,
                        };

            return query.ToList();
        }

        /// <summary>
        /// Retrieves a single complaint entity including navigation properties.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to retrieve.</param>
        /// <returns>The <see cref="Complaints"/> entity with details, or null if not found.</returns>
        public Complaints? GetComplaintWithDetails(int complaintId)
        {
            return this.context.Complaints
                .Include(c => c.Post)
                .FirstOrDefault(c => c.Id == complaintId);
        }

        /// <summary>
        /// Deletes a specific complaint from the database.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to delete.</param>
        public void DeleteComplaint(int complaintId)
        {
            var complaint = this.context.Complaints.Find(complaintId);
            if (complaint != null)
            {
                this.context.Complaints.Remove(complaint);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a new complaint record to the database.
        /// </summary>
        /// <param name="reasonText">The text content of the complaint.</param>
        /// <param name="postId">The ID of the post the complaint is against.</param>
        /// <param name="complainantId">The ID of the user who filed the complaint.</param>
        public void AddComplaint(string reasonText, int postId, int complainantId)
        {
            var newComplaint = new Complaints
            {
                Text = reasonText,
                PostId = postId,
                ComplainantId = complainantId,
                Date = DateTime.UtcNow,
            };

            this.context.Complaints.Add(newComplaint);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Approves a complaint, which results in the deletion of the associated post,
        /// its related genre mappings, and the complaint record itself.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to approve and process.</param>
        /// <exception cref="Exception">Thrown if the complaint is not found.</exception>
        public void ApproveComplaint(int complaintId)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var complaint = this.context.Complaints
                .Include(c => c.Post)
                .ThenInclude(p => p.BookGenres)
                .FirstOrDefault(c => c.Id == complaintId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (complaint == null)
            {
                throw new Exception("Скаргу не знайдено.");
            }

            // Remove BookGenres (Many-to-Many join table entries)
            if (complaint.Post != null && complaint.Post.BookGenres.Any())
            {
                this.context.BookGenres.RemoveRange(complaint.Post.BookGenres);
            }

            // Remove the Post itself
            if (complaint.Post != null)
            {
                this.context.Posts.Remove(complaint.Post);
            }

            // Remove the Complaint record
            this.context.Complaints.Remove(complaint);

            this.context.SaveChanges();
        }
    }
}