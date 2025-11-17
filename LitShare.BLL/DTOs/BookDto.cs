namespace LitShare.BLL.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string DealType { get; set; }
        public string Location { get; set; }
        public string ImagePath { get; set; }
        public int UserId { get; set; }
    }
}