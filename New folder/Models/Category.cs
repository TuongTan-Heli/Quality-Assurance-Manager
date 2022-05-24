using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Category
    {
        [Key]
        public int CateId { get; set; }

        [Required]
        public string CateName { get; set; }
        
        [Required]
        [StringLength(200)]
        public string CateDescription { get; set; }
        
    }
}