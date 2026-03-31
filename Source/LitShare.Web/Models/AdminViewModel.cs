namespace LitShare.Web.Models
{
    using LitShare.BLL.DTOs;

    public class AdminViewModel
    {
        public List<ComplaintDto> Complaints { get; set; } = new List<ComplaintDto>();

        public int TotalCount => this.Complaints.Count;
    }
}