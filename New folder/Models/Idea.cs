using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Idea
    {
       

        [Key]
        public int IdeaId { get; set; }

        [Required]
        public string IdeaTitle { get; set; }

        [Required]
        [StringLength(100)]
        public string IdeaDescription { get; set; }

        [Required]
        public string IdeaContent { get; set; }

        [Required]
        public DateTime Created_date { get; set; }

        [Required]
        public DateTime Last_modified { get; set; }
        public bool Anonymous { get; set; }
       
        public virtual  ApplicationUser Author { get; set; }
        public virtual Department Department { get; set; }
        public virtual Category Category { get; set; }
        public virtual Submission Submission { get; set; }
        public virtual ICollection<Comment> Comments {get;set;}
        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<View> Views {get;set;}
    }
}