namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.DTOs;

    public interface IRegisterService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
    }
}