using Microsoft.EntityFrameworkCore;

namespace CommonUtilities.Models;

public class DB : DbContext
{
    public DB(DbContextOptions<DB> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        List<UserRoles> userRoles = EnumHelper.ToList<UserRoles>();
        // Seed Roles
        builder.Entity<Role>()
            .HasData(userRoles.Select((role, index) => new Role { Id = index + 1, Name = role.ToString() }));
    }

    /**
     * @ModelName: Product
     * @TableName: Products
     *
     * Remember Table Name Must Add "s" at the end
     */
    //public DbSet<Product> Products { get; set; }
}