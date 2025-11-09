using AioStudy.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AioStudy.Api.Data

{
    public class ApplicationDbContext : DbContext
    {
         public DbSet<User> users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
    }
}
