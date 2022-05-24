using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Submission
    {
        [Key]
        public int SubId { get; set; }

        [Required]
        public string SubName { get; set; }

        [Required]
        [StringLength(100)]
        public string SubDescription { get; set; }
        [Required]
        [DataType(DataType.Date)]
      
        public DateTime Open_date { get; set; }
        [Required]
        [DataType(DataType.Date)]
      
        public DateTime Closure_date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0t}")]
        public DateTime Final_closure_date { get; set; }
      
    }
}