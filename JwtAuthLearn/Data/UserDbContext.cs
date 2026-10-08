using JwtAuthLearn.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthLearn.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
