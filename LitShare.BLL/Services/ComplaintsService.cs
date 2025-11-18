// -----------------------------------------------------------------------
// <copyright file="ComplaintsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LitShare.BLL.DTOs;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Надає бізнес-логіку для керування скаргами (Complaints).
    /// </summary>
    public class ComplaintsService
    {
        // 1. Виправлення SA1309: Іменування приватного поля
        private readonly LitShareDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintsService"/> class.
        /// </summary>
        /// <param name="context">The optional database context. If null, a new context is created.</param>
        public ComplaintsService(LitShareDbContext? context = null)
        {
            // 2. Виправлення SA1101: видалено this.
            this.context = context ?? new LitShareDbContext();
        }

        /// <summary>
        /// Retrieves a list of all complaints, mapped to <see cref="ComplaintDto"/> with aggregated details.
        /// </summary>
        /// <returns>A list of <see cref="ComplaintDto"/> objects.</returns>
        public List<ComplaintDto> GetAllComplaints()
        {
            // Використовуємо _context
            var query = from c in this.context.Complaints.AsNoTracking() // Додаємо AsNoTracking для читання
                        join p in this.context.Posts.AsNoTracking() on c.PostId equals p.Id
                        join u in this.context.Users.AsNoTracking() on c.ComplainantId equals u.Id
                        select new ComplaintDto
                        {
                            Text = c.Text,
                            BookTitle = p.Title,
                            UserName = u.Name,
                            Date = c.Date,
                        };

            return query.ToList();
        }

        /// <summary>
        /// Retrieves a single complaint entity by ID, including its associated Post.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to retrieve.</param>
        /// <returns>The <see cref="Complaints"/> entity, or null if not found.</returns>
        public Complaints? GetComplaintWithDetails(int complaintId)
        {
            // Використовуємо _context
            return this.context.Complaints
                .Include(c => c.Post)
                .FirstOrDefault(c => c.Id == complaintId);
        }

        /// <summary>
        /// Deletes a complaint entity from the database by its ID.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to delete.</param>
        public void DeleteComplaint(int complaintId)
        {
            // Використовуємо _context
            var complaint = this.context.Complaints.Find(complaintId);

            if (complaint != null)
            {
                this.context.Complaints.Remove(complaint);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a new complaint to the database.
        /// </summary>
        /// <param name="reasonText">The text description of the complaint.</param>
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
    }
}