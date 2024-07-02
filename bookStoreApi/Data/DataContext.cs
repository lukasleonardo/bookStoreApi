using bookStoreApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace bookStoreApi.Data
{
    public class DataContext:IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options) { }
       
        public DbSet<Book> books { get; set; }


    }
}
