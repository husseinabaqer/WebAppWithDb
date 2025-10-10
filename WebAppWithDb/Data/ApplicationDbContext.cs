using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data.Tables;
using WebAppWithDb.Models;

namespace WebAppWithDb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<CoveredCity> CoveredCities => Set<CoveredCity>();
        public DbSet<Request> Requests => Set<Request>();

    }
}
