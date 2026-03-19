using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;

namespace LitShare.DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly LitShareDbContext context;

        public PostRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Posts post)
        {
            await this.context.Posts.AddAsync(post);

            await this.context.SaveChangesAsync();
        }
    }
}