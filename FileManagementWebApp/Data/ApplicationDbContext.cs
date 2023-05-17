using FileManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManagementWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base() { }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<FileInformation> FileInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileInformation>()
                .ToTable("FileInfos")
                .HasKey(f => f.Id);
        }
    }
}
