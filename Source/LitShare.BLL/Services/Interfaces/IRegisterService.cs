namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IRegisterService
    {
        Task<Result<bool>> RegisterAsync(RegisterDto dto);
    }
}