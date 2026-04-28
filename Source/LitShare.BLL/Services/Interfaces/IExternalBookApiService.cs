namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.DTOs;

    public interface IExternalBookApiService
    {
        Task<VolumeInfo?> GetBookDetailsAsync(string bookTitle);
    }
}