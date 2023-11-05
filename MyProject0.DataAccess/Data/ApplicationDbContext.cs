using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProject0.Models;

namespace MyProject0.DataAccess.Data
{
    // These are the basic syntaxes for using the entity framework core
    // 1) Inherit the DbContext class (This is like the root class) inside your own DbContext file.
    // 2) Adding the DbContextOptions inside the constructor and passing the same variable to the base class (DbContext here).
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
		public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base (options)
		{

		}

		// We have added the connection string that will link the database to our project, we have made a Model
		// and now we want a table of type Model inside our database.
		// Here we are telling EF Core to add a table inside our database of type Model.

		public DbSet <Catagory> Catagories { get; set; }
        public DbSet<Product> Product { get; set; }

        // We can also tell EF Core to add some speciic data inside a table
        // This is called seeding of the data, we are adding rows from here to the database.

        protected override void OnModelCreating (ModelBuilder modelbuilder)
		{
            // We need to add this line for the sake of configuratin of the IdentityDbContext
            base.OnModelCreating(modelbuilder);

			modelbuilder.Entity<Catagory>().HasData(
				new Catagory { ID = 1, Name = "SciFi",DisplayOrder=1},
                new Catagory { ID = 2, Name = "Drama", DisplayOrder = 2 },
                new Catagory { ID = 3, Name = "Funky", DisplayOrder = 3 }
                );
            modelbuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Fortune of Time",
                    Author = "Billy Spark",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CatagoryId = 1,
                    ImageUrl=""
                },
                new Product
                {
                    Id = 2,
                    Title = "Dark Skies",
                    Author = "Nancy Hoover",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CatagoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CatagoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CatagoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CatagoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CatagoryId = 1,
                    ImageUrl = ""
                }
                );
        }
		// Whenever anything that we need to update from code side to the data side, migration command will be used
		// Always remember that.
	}
}

