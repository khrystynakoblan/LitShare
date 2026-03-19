using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LitShare.Web.Models
{
    public class CreatePostModel
    {
        [Display(Name = "Назва книги")]
        [Required(ErrorMessage = "Введіть назву книги")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Автор книги")]
        [Required(ErrorMessage = "Введіть автора")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Жанр")]
        public int GenreId { get; set; }

        public List<SelectListItem>? Genres { get; set; }

        [Display(Name = "Тип угоди")]
        public int DealTypeId { get; set; }

        public List<SelectListItem>? DealTypes { get; set; }

        [Display(Name = "Опис")]
        [Required(ErrorMessage = "Додайте опис")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Фото")]
        public IFormFile? ImageFile { get; set; }
    }
}