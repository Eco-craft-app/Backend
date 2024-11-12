using Microsoft.EntityFrameworkCore;
using Modules.Projects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Database;
public class ProjectsDbContext(DbContextOptions<ProjectsDbContext> opt) : DbContext(opt)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        modelBuilder.Entity<Comment>()
          .HasIndex(c => c.ProjectId)
          .HasDatabaseName("IX_Comments_ProjectId");


        modelBuilder.Entity<Like>()
            .HasIndex(l => l.UserId)
            .HasDatabaseName("IX_Likes_UserId");


        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.ProjectId })
            .HasDatabaseName("IX_Likes_UserId_ProjectId");

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Title)
            .HasDatabaseName("IX_Projects_Title");

        modelBuilder.Entity<Project>()
            .HasIndex(p => new { p.UserId, p.CreatedAt })
            .HasDatabaseName("IX_Projects_UserId_CreatedAt");
        base.OnModelCreating(modelBuilder);
    }
}
