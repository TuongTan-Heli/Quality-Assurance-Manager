using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class View
    {
        [Key]
        public int ViewId { get; set; }

        [Required]
        public DateTime Last_visited { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Idea Idea { get; set; }

    }
}