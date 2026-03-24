namespace LitShare.BLL.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IGenreService
    {
        Task<Result<List<GenreDto>>> GetAllGenresAsync();
    }
}