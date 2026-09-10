using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using test.Models;


namespace test.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection")
        {
            Database.SetInitializer<AppDbContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
    }
}