using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class LockDbContext : DbContext
    {
        public LockDbContext(DbContextOptions<LockDbContext> options)
            : base(options)
        {

        }

        public DbSet<Lock> Locks { get; set; }
    }
}
