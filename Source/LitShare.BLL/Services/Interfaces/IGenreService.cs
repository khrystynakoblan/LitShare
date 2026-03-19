namespace LitShare.BLL.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;

    public interface IGenreService
    {
        Task<IEnumerable<GenreDto>> GetAllGenresAsync();
    }
}