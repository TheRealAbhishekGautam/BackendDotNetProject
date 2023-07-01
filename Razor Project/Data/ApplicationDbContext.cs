using System;
using Microsoft.EntityFrameworkCore;
using Razor_Project.Models;

namespace Razor_Project.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
		{
		}

		public DbSet <Catagory> Catagories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Catagory>().HasData(
				new Catagory { ID = 1, Name = "SciFi", DisplayOrder = 1 },
				new Catagory { ID = 2, Name = "Drama", DisplayOrder = 2 },
				new Catagory { ID = 3, Name = "Funky", DisplayOrder = 3 }
				);
		}
	}
}
