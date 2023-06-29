using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Razor_Project.Models
{
	public class Catagory
    {
        [Key]                                  
        public int ID { get; set; }

        [Required]                              
        [DisplayName("Catagory Name")]        
        [MaxLength(30)]                         
        public String Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "The range allowed is between 1-100")]    
        public int DisplayOrder { get; set; }
	}
}

