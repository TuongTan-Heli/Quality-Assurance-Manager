using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Reaction
    {
        [Key]
        public int ReactId { get; set; }
        //True for like, false for unlike
        public bool React { get; set; }
     
        public virtual Idea Idea { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}