using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASP.NETProject.Models;

namespace ASP.NETProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ASP.NETProject.Models.Article>? Article { get; set; }
        public DbSet<ASP.NETProject.Models.Comment>? Comment { get; set; }
    }
}