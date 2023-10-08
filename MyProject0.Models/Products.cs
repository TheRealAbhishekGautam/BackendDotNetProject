using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject0.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }
        [Required]
        public String ISBN { get; set; }
        [Required]
        public String Author { get; set; }

        [Required]
        [Display(Name = "List Price")]
        [Range(1,100)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 100)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 100)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 100)]
        public double Price100 { get; set; }

        // CatagoryId is a property which will contain the Id of the catagory 
        public int CatagoryId { get; set; }

        // Here we are actually telling to store the value of foreign key from Catagory property to CatagoryId property defined here.
        [ForeignKey("CatagoryId")]
        [ValidateNever]
        public Catagory Catagory { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

    }
}

