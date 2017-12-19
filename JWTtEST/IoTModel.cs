using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    public class IoTContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=JWTtEST.ConsoleApp.NewDb;Trusted_Connection=True;");
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Registration> Registrations { get; set; }
    }
}
