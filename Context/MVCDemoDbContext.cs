using EntityFrameworkPractical.Models;
using EntityFrameworkPractical.Models.Domain;
using Microsoft.EntityFrameworkCore;

public class MVCDemoDbContext : DbContext
{
  public MVCDemoDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
}
