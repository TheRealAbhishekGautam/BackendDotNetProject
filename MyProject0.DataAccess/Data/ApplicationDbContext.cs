using System;
using Microsoft.EntityFrameworkCore;
using MyProject0.Models;

namespace MyProject0.Data
{
    // These are the basic syntaxes for using the entity framework core
    // 1) Inherit the DbContext class (This is like the root class) inside your own DbContext file.
    // 2) Adding the DbContextOptions inside the constructor and passing the same variable to the base class (DbContext here).
    public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base (options)
		{

		}

		// We have added the connection string that will link the database to our project, we have made a Model
		// and now we want a table of type Model inside our database.
		// Here we are telling EF Core to add a table inside our database of type Model.

		public DbSet <Catagory> Catagories { get; set; }

		// We can also tell EF Core to add some speciic data inside a table
		// This is called seeding of the data, we are adding rows from here to the database.

		protected override void OnModelCreating (ModelBuilder modelbuilder)
		{
			modelbuilder.Entity<Catagory>().HasData(
				new Catagory { ID = 1, Name = "SciFi",DisplayOrder=1},
                new Catagory { ID = 2, Name = "Drama", DisplayOrder = 2 },
                new Catagory { ID = 3, Name = "Funky", DisplayOrder = 3 }
                );
		}
		// Whenever anything that we need to update from code side to the data side, migration command will be used
		// Always remember that.
	}
}

