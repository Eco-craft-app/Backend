using Microsoft.EntityFrameworkCore;
using Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Database;
public class UsersProfilesDbContext(DbContextOptions<UsersProfilesDbContext> opt) : DbContext(opt) 
{
    public DbSet<UserProfile> UsersProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
