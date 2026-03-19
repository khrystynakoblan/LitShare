namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.DTOs;

    public interface ILoginService
    {
        Task<bool> LoginAsync(LoginDto dto);
    }
}