using Microsoft.EntityFrameworkCore;
using UsersAPI.Models;

namespace UsersAPI.Data;

public class UsersDBContext(DbContextOptions<UsersDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(x => x.address).OwnsOne(x => x.geo);
        modelBuilder.Entity<User>().OwnsOne(x => x.company);
    }
}
