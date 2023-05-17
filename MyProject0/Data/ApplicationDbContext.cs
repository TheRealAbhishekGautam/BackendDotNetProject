using System;
using Microsoft.EntityFrameworkCore;

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
	}
}

