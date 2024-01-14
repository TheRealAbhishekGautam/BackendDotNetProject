using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject0.Models
{
    // We are extending our IdentityUser to add our additional properties to the database.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public int? CompanyId  { get; set; }

        // When a user registers we have to specify wether the user is a
        // Regular User or a Company User.
        [ValidateNever]
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
