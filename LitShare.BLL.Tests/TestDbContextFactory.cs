using LitShare.DAL;
using Microsoft.EntityFrameworkCore;

namespace LitShare.BLL.Tests
{
    public static class TestDbContextFactory
    {
        public static LitShareDbContext Create()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new LitShareDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}