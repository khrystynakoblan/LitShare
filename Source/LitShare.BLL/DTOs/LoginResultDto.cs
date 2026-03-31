namespace LitShare.BLL.DTOs
{
    using LitShare.DAL.Models;

    public class LoginResultDto
    {
        public int UserId { get; set; }

        public RoleType Role { get; set; }
    }
}