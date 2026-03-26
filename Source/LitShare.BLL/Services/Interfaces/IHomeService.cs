namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IHomeService
    {
        Task<Result<List<PostCardDto>>> GetAllPostsAsync();

        Task<Result<List<PostCardDto>>> GetFilteredPostsAsync(PostFilterDto filter);
    }
}