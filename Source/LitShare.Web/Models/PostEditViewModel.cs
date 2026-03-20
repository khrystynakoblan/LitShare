namespace LitShare.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using LitShare.DAL.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// ViewModel for editing a book post.
    /// </summary>
    public class PostEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва книги обов'язкова")]
        [Display(Name = "Назва книги")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Автор обов'язковий")]
        [Display(Name = "Автор книги")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Display(Name = "Тип угоди")]
        public DealType DealType { get; set; }

        public string? PhotoUrl { get; set; }

        [Display(Name = "Жанр")]
        public int SelectedGenreId { get; set; }

        public IFormFile? NewPhoto { get; set; }

        public SelectList? Genres { get; set; }

        public SelectList? DealTypes { get; set; }
    }
}