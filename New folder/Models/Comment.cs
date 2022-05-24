
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }
        public bool Anonymous { get; set; }
    
        [Required]
        public DateTime Created_date { get; set; }
        public virtual ApplicationUser Author { get; set; }
        [Required]
        public DateTime Last_modified { get; set; }
        public virtual Comment Reply { get; set; }
        public virtual ICollection<Comment> Replys { get; set;}
        public virtual Idea Idea { get; set; }
    }
}