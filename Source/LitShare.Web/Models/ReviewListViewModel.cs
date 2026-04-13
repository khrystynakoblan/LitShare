namespace LitShare.Web.Models
{
    using System.Collections.Generic;
    using LitShare.BLL.DTOs;

    public class ReviewListViewModel
    {
        public int ReviewedUserId { get; set; }

        public string ReviewedUserName { get; set; } = string.Empty;

        public double AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public bool CanLeaveReview { get; set; }

        public IEnumerable<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }
}