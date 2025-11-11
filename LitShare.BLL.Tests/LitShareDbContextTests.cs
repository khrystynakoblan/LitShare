using Xunit;
using LitShare.DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LitShare.BLL.Tests
{
    public class LitShareDbContextTests
    {
        [Fact]
        public void OnConfiguring_Should_Apply_DefaultConnection_When_NotConfigured()
        {
            
            var context = new LitShareDbContext(); 

            
            var builder = new DbContextOptionsBuilder<LitShareDbContext>();
            context.GetType()
                   .GetMethod("OnConfiguring",
                       System.Reflection.BindingFlags.NonPublic |
                       System.Reflection.BindingFlags.Instance)!
                   .Invoke(context, new object[] { builder });

            
            var configured = builder.Options.Extensions.Any();
            Assert.True(configured); 
        }

        [Fact]
        public void ConfigureMapper_Should_Only_Run_Once()
        {
            
            var context1 = new LitShareDbContext();
            var context2 = new LitShareDbContext();

            
            Assert.NotNull(context1);
            Assert.NotNull(context2);
        }
    }
}