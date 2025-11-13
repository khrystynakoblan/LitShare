using LitShare.DAL.Models;
using Xunit;

namespace LitShare.BLL.Tests
{
    public class BookGenresTests
    {
        [Fact]
        public void BookGenres_Should_Set_And_Get_All_Properties()
        {
            
            var genre = new Genres { id = 10, name = "Drama" };
            var post = new Posts { id = 20, title = "Book", author = "Author" };

            
            var bookGenre = new BookGenres
            {
                post_id = post.id,
                genre_id = genre.id,
                Post = post,
                Genre = genre
            };

            
            Assert.Equal(20, bookGenre.post_id);
            Assert.Equal(10, bookGenre.genre_id);
            Assert.Equal("Book", bookGenre.Post.title);
            Assert.Equal("Drama", bookGenre.Genre.name);
        }
    }
}