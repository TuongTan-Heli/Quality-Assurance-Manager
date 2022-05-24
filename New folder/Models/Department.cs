using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Department
    {
        [Key]
        public int DepartId { get; set; }

        [Required]
        public string DepartName { get; set; }  
    }
}