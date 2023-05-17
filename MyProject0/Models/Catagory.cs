using System;
using System.ComponentModel.DataAnnotations;

namespace MyProject0.Models
{
	// What we are actually doing is creating a class (Model) here in the Project and we will tell
	// Entity Framework to create the same table inside the database given.
	// Now the question arrises how to tell ef core that this column will be the primary key.?
	// For that Annotations comes into the picture.
	// Now for telling the project to make this table we need to install some Nuget packages to our project.
	// Microsoft Entity Framework Core, Microsoft EFCore SQLServer, Microsoft EFCore Tools
	// Always try to install all of the nuget packages of the same version (7.0.5 in this case).
	public class Catagory
	{
		[Key] // Primary Key, will not have the duplicate values
		public int ID { get; set; }
		[Required] // NOT NULL inside the sql tables
		public String Name { get; set; }
		public int DisplayOrder { get; set; }
	}
}