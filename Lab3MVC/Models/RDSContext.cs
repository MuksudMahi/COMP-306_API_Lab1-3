using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Lab3MVC.Models
{
    public class RDSContext: DbContext
    {
        public RDSContext(DbContextOptions<RDSContext> options) : base(options)
        {

        }
        public DbSet<User> Courses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");

        }

    }

}
