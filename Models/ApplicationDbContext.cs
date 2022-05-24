using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication2.Models.Idea> Ideas { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.Department> Departments { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.Document> Documents { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.Reaction> Reactions { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.Submission> Submissions { get; set; }

        public System.Data.Entity.DbSet<WebApplication2.Models.View> Views { get; set; }
    }
}