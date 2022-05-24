using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Document
    {
        [Key]
        public int FileId { get; set; }

        [Required]
        public string FilePath { get; set; }
        public string FileName { get; set; }

        [Required]
        public DateTime Create_date { get; set; }

        [Required]
        public DateTime Last_modified { get; set; }

        public virtual Idea Idea { get; set; }
    }
}