using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Text;
using WebApplication2.Models;
using System.IO;
using System.Drawing;

namespace WebApplication2.Controllers
{   [Authorize]
    public class IdeasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ideas
        public ActionResult Index(int? i,string Sort,string search)
        {
            
           
            List<Document> documents = db.Documents.Include(x => x.Idea).ToList();
            ViewBag.documents = documents;
            List<Idea> Ideas = db.Ideas.Include(x=>x.Author).Include(y=>y.Category).Include(z=>z.Department).Include(v=>v.Submission).ToList();
            foreach(Idea idea in Ideas)
            {
               
                idea.Comments = db.Comments.Include(x => x.Reply).Include(y=>y.Author).ToList();
                idea.Comments=idea.Comments.Where(x => x.Idea == idea&&x.Reply==null).ToList();
                foreach (Comment comment in idea.Comments)
                {
                  RecursionGetComments(comment);
                }
            }
            if(search != null)
            {
                Ideas=Ideas.Where(x=>x.IdeaTitle.Contains(search)).ToList();
            }
            if(Sort != null)
            {
                switch (Sort)
                {
                    case "Lastest":
                        Ideas = Ideas.OrderByDescending(x => x.Created_date).ToList();
                        break;
                    case "Most Liked":
                        foreach(Idea idea in Ideas)
                        {
                            
                            List<Reaction> reactions = db.Reactions.ToList();
                            idea.Reactions = reactions.Where(x => x.React == true&&x.Idea==idea).ToList();
                            
                        }
                        Ideas=Ideas.OrderByDescending(x=>x.Reactions.Count).ToList();
                       
                        break;
                    case "Most Viewed":
                        foreach (Idea idea in Ideas)
                        {
                            List<View> views = db.Views.ToList();
                            idea.Views = views.Where(x => x.Idea == idea).ToList();

                        }
                        Ideas=Ideas.OrderByDescending(x => x.Views.Count).ToList();
                        break;
                    case "Most Comments":
                        foreach (Idea idea in Ideas)
                        {
                            List<Comment> comments = db.Comments.ToList();
                            idea.Comments = comments.Where(x => x.Idea == idea).ToList();

                        }
                        Ideas=Ideas.OrderByDescending(x => x.Comments.Count).ToList();
                       
                        break;
                    default:
                        break;
                }
                    
            }
            return View(Ideas.ToPagedList(i ?? 1, 5));
          
        }
        List<Comment> GetComments(int Id) {
            List<Comment> comments = db.Comments.Include(x => x.Reply).ToList();
            comments = comments.Where(x => x.Reply == db.Comments.Find(Id)).ToList();

            return comments;
        }
        public void RecursionGetComments(Comment comment)
        {
            comment.Replys = GetComments(comment.CommentId);
            foreach(var reply in comment.Replys)
            {
                RecursionGetComments(reply);
            }
        }

        // GET: Ideas/Details/5
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Include(x=>x.Category).Include(y=>y.Author).Include(z=>z.Department).Include(c=>c.Submission).FirstOrDefault(v=>v.IdeaId==id);
            idea.Comments = db.Comments.Include(x => x.Reply).Include(y => y.Author).ToList();
            idea.Comments = idea.Comments.Where(x => x.Idea == idea && x.Reply == null).ToList();
            foreach (Comment comment in idea.Comments)
            {
                RecursionGetComments(comment);
            }
            ViewBag.Categories = new SelectList(db.Categories, "CateId", "CateName");
            List<View> views = db.Views.Include(x=>x.Idea).Include(y=>y.User).ToList();
            idea.Views = views.Where(x => x.Idea == idea).ToList();

            List<Reaction> reactions = db.Reactions.Include(x => x.User).Include(y=>y.Idea).ToList();
            idea.Reactions = reactions.Where(x => x.Idea == idea).ToList();
            ViewBag.Documents = db.Documents.Include(x => x.Idea).ToList();
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // GET: Ideas/Create
        public ActionResult Create()
        {
            
            ViewBag.Categories = new SelectList(db.Categories, "CateId", "CateName");
            return View();
        }

        // POST: Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Idea idea, int Categories,HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                idea.Author = db.Users.Find(User.Identity.GetUserId());
                idea.Category = db.Categories.Find(Categories);
                idea.Department = idea.Author.Department;
                idea.Created_date = DateTime.Now;
                idea.Last_modified = DateTime.Now;
                List<Submission> submissions = db.Submissions.ToList();
                if(submissions.Where(x => x.Open_date <= DateTime.Now && DateTime.Now <= x.Closure_date).ToList().Count != 0)
                {
                    Submission currentSub = submissions.Where(x => x.Open_date <= DateTime.Now && DateTime.Now <= x.Closure_date).ToList()[0];
                    idea.Submission = currentSub;
                }
                else
                {
                    return Content("<script>" +
                        "alert('There is no open submission recently, please come back later.');" +
                        "history.back();" +
                        "</script>");

                }

                db.Ideas.Add(idea);
                db.SaveChanges();
                Idea idea1 = db.Ideas.ToList().Last();
                SendEmail(idea1,1);
                if (file != null)
                {//Create new file
                    Document document = new Document();
                    document.Idea = idea;
                    document.FilePath = "/Files/";
                    document.Create_date = DateTime.Now;
                    document.Last_modified = DateTime.Now;
                    String fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    String extension = Path.GetExtension(file.FileName);
                    fileName = fileName + extension;
                    document.FileName = fileName;
                    db.Documents.Add(document);
                 
                    file.SaveAs(Server.MapPath("~/Files/" + fileName));
                }
               
                db.SaveChanges();
                TempData["AlertMessage"] = "Create new idea successfully...!";
                return RedirectToAction("Index","Home");
            }


            return RedirectToAction("Index", "Home");
        }


        // GET: Ideas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit (Idea idea,int? Categories)
        {
           
            if (ModelState.IsValid)
            {
                string userId=User.Identity.GetUserId();
                db.Ideas.Find(idea.IdeaId).IdeaContent = idea.IdeaContent;
                db.Ideas.Find(idea.IdeaId).Department = db.Users.Find(userId).Department;
                db.Ideas.Find(idea.IdeaId).IdeaTitle = idea.IdeaTitle;
                db.Ideas.Find(idea.IdeaId).IdeaDescription = idea.IdeaDescription;
                db.Ideas.Find(idea.IdeaId).Last_modified = DateTime.Now;
                if(Categories != null||Categories!=0)
                {
                    db.Ideas.Find(idea.IdeaId).Category = db.Categories.Find(Categories);
                }
                
                db.SaveChanges();
                TempData["AlertMessage"] = "Update idea successfully...!";

                return RedirectToAction("Index");
            }
            return View(idea);
        }

        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Idea idea = db.Ideas.Find(id);

            DeleteComponent(id);
            db.Ideas.Remove(idea);
            db.SaveChanges();
            TempData["AlertMessage"] = "Delete idea successfully...!";

            return RedirectToAction("Index");
        }

        public void DeleteComponent(int id)
        {
            Idea idea=db.Ideas.Find(id);
            //Comment
       
        

                idea.Comments = db.Comments.Include(x => x.Reply).Include(y => y.Author).ToList();
                idea.Comments = idea.Comments.Where(x => x.Idea == idea&&x.Reply==null).ToList();
                List<Comment> comments = idea.Comments.ToList();
                foreach (Comment comment in idea.Comments)
                {
                    comments.AddRange(RecursionGetListComments(comment));
                }
                db.Comments.RemoveRange(comments);
           
            //View
            List<View> views=db.Views.ToList();
            views = views.Where(x => x.Idea == idea).ToList();
            db.Views.RemoveRange(views);
            //Reaction
            List<Reaction> reactions = db.Reactions.ToList();
            reactions = reactions.Where(x => x.Idea == idea).ToList();
            db.Reactions.RemoveRange(reactions);

            //File
            List<Document> documents = db.Documents.ToList();
            Document document =documents.FirstOrDefault(x=>x.Idea == idea);
            if(document != null)
            {
                db.Documents.Remove(document);
            }
          
              db.SaveChanges();
        }
        public List<Comment> RecursionGetListComments(Comment comment)
        {

            List<Comment> comments = new List<Comment>();
            comment.Replys = GetComments(comment.CommentId);
            comments.AddRange(comment.Replys);
            foreach (var reply in comment.Replys)
            {
                RecursionGetListComments(reply);
            }
            return comments;
        }
        public void SendEmail(Idea idea,int cases)
        {
            string subject="";
            string body="";
            string link = Request.UrlReferrer.ToString().Substring(0, 30);
            ApplicationUser user;
            if (cases == 1)//case send email when create idea
            {
              
                List<ApplicationUser> users =db.Users.Include(x=>x.Department).ToList();

                user = users.Where(x => x.Department == idea.Department).FirstOrDefault();
                subject=subject+($"New idea submitted from {idea.Author.UserName}");
                body=body+($"A new idea has been submitted from {idea.Author.UserName} with idea tile is {idea.IdeaTitle}\n");
                body = body + ($"Please check it on {link}details/{idea.IdeaId} \n");
                body = body + ($"This is an automatic message sent from the QA System of group 4");

              


            }
            else//case send email when comment
            {

                
                user = idea.Author;
                subject=subject+($"New comment for your idea ");
                body = body + ($"A new comment has been made on your idea\n");
                body = body + ($"Please check it on {link}details/{idea.IdeaId}\n");
                body = body + ($"This is an automatic message sent from the QA System of group 4");

             
            }
            
            subject.Replace('\r', ' ').Replace('\n', ' ');
            body.Replace('\r', ' ').Replace('\n', ' ');
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("mail here", "pass here"),
                EnableSsl = true
            };

            
         
        

            client.Send("mail here", user.Email, subject, body);


            return;
        }
        [HttpPost]
        public ActionResult Comment(string Comment,int IdeaId,bool Anonymous,int CommentId)
        {
            Comment comment = new Comment();
            comment.Anonymous = Anonymous;
            comment.Content = Comment;
            comment.Author = db.Users.Find(User.Identity.GetUserId());
            comment.Created_date = DateTime.Now;
            comment.Last_modified = DateTime.Now;
            //Check if it is a comment or reply on comment
            if (CommentId ==0)
            {
                comment.Idea = db.Ideas.Find(IdeaId);
               
                
            }
            else
            {
                comment.Idea = db.Comments.Find(CommentId).Idea;
                comment.Reply = db.Comments.Find(CommentId);
             
            }
           db.Comments.Add(comment);
            SendEmail(comment.Idea, 2);
             db.SaveChanges();
            TempData["AlertMessage"] = "Add comment successfully...!";

            return RedirectToAction("Index", "Ideas");
        }
        [HttpPost]
        public ActionResult React(string React, int IdeaId)
        {
          
            bool Reaction;
            if (React.ToString() != "null")
            {
                if (React.ToString() == "0")
                {
                    Reaction = false;
                }
                else
                {
                    Reaction = true;
                }
                Reaction react = new Reaction();
                react.React = Reaction;
                react.Idea = db.Ideas.Find(IdeaId);
                react.User = db.Users.Find(User.Identity.GetUserId());
                //If user already react to post
                List<Reaction> reactions = db.Reactions.ToList();
                if (reactions.Where(x => x.User == react.User && x.Idea == react.Idea).ToList().Count != 0)
                {
                    Reaction react1 = reactions.Where(x => x.User == react.User && x.Idea == react.Idea).ToList()[0];
                    react1.React = react.React;
                    db.Entry(react1).State = EntityState.Modified;


                }
                else
                {
                    db.Reactions.Add(react);
                }
            }
            else
            {
                Reaction react = new Reaction();
                
                react.Idea = db.Ideas.Find(IdeaId);
                react.User = db.Users.Find(User.Identity.GetUserId());
                List<Reaction> reactions = db.Reactions.ToList();
                Reaction react1 = reactions.Where(x => x.User == react.User && x.Idea == react.Idea).ToList()[0];
                db.Reactions.Remove(react1);
            }
           
           
           
            
            db.SaveChanges();
            return RedirectToAction("Index", "Ideas");
        }
        public ActionResult DownloadCSV(string Submission)
        {
            Submission submission;
           
            List<Idea> ideas = db.Ideas.Include(x => x.Category).Include(x => x.Department).Include(x => x.Author).ToList();
            if (Submission == null)
            {
                
                List<Submission> submissions = db.Submissions.ToList();
                submission = submissions.FirstOrDefault(x => x.Open_date <= DateTime.Now && DateTime.Now <= x.Closure_date);
                ideas = ideas.Where(x => x.Submission == submission).ToList();
            }
            else
            {
         
                submission = db.Submissions.FirstOrDefault(x => x.SubName == Submission);
                ideas = ideas.Where(x => x.Submission == submission).ToList();
            }
            
            var builder = new StringBuilder();
            builder.AppendLine("Id,Title,Description,Content,Created date,Last modified,Category,Author,Document");
            String link = Request.UrlReferrer+"GetFile?IdeaId=";
          

            foreach(Idea idea in ideas)
            {
               
                List<Document> documents = db.Documents.Include(x => x.Idea).ToList();
                Document document = documents.Where(x => x.Idea == idea).FirstOrDefault();
                if(document != null)
                {
                    builder.AppendLine($"{idea.IdeaId},{idea.IdeaTitle},{idea.IdeaDescription},{idea.IdeaContent},{idea.Created_date},{idea.Last_modified},{idea.Category.CateName},{idea.Author.UserName},{link}{idea.IdeaId}");
                }
                else
                {
                    builder.AppendLine($"{idea.IdeaId},{idea.IdeaTitle},{idea.IdeaDescription},{idea.IdeaContent},{idea.Created_date},{idea.Last_modified},{idea.Category.CateName},{idea.Author.UserName}");
                }
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Ideas.csv");
        }
        public JsonResult AddView(int IdeaId)
        {
            View view= new View();
            view.Idea = db.Ideas.Find(IdeaId);
            view.User = db.Users.Find(User.Identity.GetUserId());
            List<View> views = db.Views.ToList();
            view.Last_visited = DateTime.Now;
            if (views.Where(x => x.User == view.User && x.Idea == view.Idea).ToList().Count == 0)
            {
                //If not already view idea
                db.Views.Add(view);
            }
            else
            {
                View view1 = views.Where(x => x.User == view.User && x.Idea == view.Idea).ToList()[0];
                view1.Last_visited= DateTime.Now;
                db.Entry(view1).State = EntityState.Modified;
            }

            return Json(db.SaveChanges());
        }
        [HttpGet]
        public ActionResult GetFile(int IdeaId)
        {
            
            List<Document> documents = db.Documents.ToList();
            if(documents.Where(x => x.Idea.IdeaId == IdeaId).ToList().Count != 0){
                Document document = documents.Where(x => x.Idea.IdeaId == IdeaId).ToList()[0];
                string url = Server.MapPath("/Files/" + document.FileName);

                return File(url, "application/force-download",document.FileName);
            }
            return null;
        }
        [HttpPost]
        public ActionResult UploadFile(int IdeaId, HttpPostedFileBase file)
        {
            if (file != null)
            {//Create new file
                Document document = new Document();
                document.Idea = db.Ideas.Find(IdeaId);
                document.FilePath = "/Files/";
                document.Create_date = DateTime.Now;
                document.Last_modified = DateTime.Now;
                String fileName = Path.GetFileNameWithoutExtension(file.FileName);
                String extension = Path.GetExtension(file.FileName);
                fileName = fileName + extension;
                document.FileName = fileName;
                List<Document> documents = db.Documents.Include(x=>x.Idea).ToList();
                if (documents.Where(x => x.Idea == document.Idea).ToList().Count != 0) {
                    Document document1 = documents.Where(x => x.Idea == document.Idea).ToList()[0];
                    document1.FilePath = document.FilePath;
                    document1.FileName = document.FileName;
                    document1.Last_modified = document.Last_modified;
                    db.Entry(document1).State = EntityState.Modified;

                }
                else
                {
                    db.Documents.Add(document);
                }
               
                    db.SaveChanges();
                TempData["AlertMessage"] = "Upload file successfully...!";

                file.SaveAs(Server.MapPath("~/Files/" + fileName));
            }
            return RedirectToAction("Index", "Ideas");
        }
        [HttpGet]
        public ActionResult DeleteFile(int IdeaId)
        {
            List<Document> documents = db.Documents.ToList();
            Idea idea = db.Ideas.Find(IdeaId);
            Document document = documents.Where(x => x.Idea == idea).ToList()[0];
            db.Entry(document).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index","Ideas"); 

        }
      
        public ActionResult Analysis(string Submission, int? i)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("QA Coordinator") || User.IsInRole("QA Manager"))
            {
                List<Idea> ideas = db.Ideas.Include(x => x.Department).Include(y => y.Submission).Include(z => z.Reactions).Include(c => c.Category).ToList();
                Submission submission;
                //view idea based on submission
                if (Submission == null)
                {
                    List<Submission> submissions = db.Submissions.ToList();
                    submission = submissions.FirstOrDefault(x => x.Open_date <= DateTime.Now && DateTime.Now <= x.Closure_date);
                    ideas = ideas.Where(x => x.Submission == submission).ToList();

                }
                else
                {
                    submission = db.Submissions.FirstOrDefault(x => x.SubName == Submission);
                    ideas = ideas.Where(x => x.Submission == submission).ToList();

                }
                ViewBag.CurrSubmission = submission;


                //ideas per department.
                List<int> Repatrition = new List<int>();
                List<string> departmentNames = ideas.Select(x => x.Department.DepartName).Distinct().ToList();
                int Authors = ideas.Select(x => x.Author.UserName).Distinct().ToList().Count();
                ViewBag.Authors = Authors;
                foreach (var department in departmentNames)
                {


                    Repatrition.Add(ideas.Count(x => x.Department.DepartName == department));
                }
                ViewBag.Repatrition = Repatrition;
                ViewBag.deparmentNames = departmentNames;

                //list idea with most comments,view, like,...
                //Most comments
                foreach (Idea idea in ideas)
                {
                    List<Comment> comments = db.Comments.ToList();
                    idea.Comments = comments.Where(x => x.Idea == idea).ToList();
                }

                ideas = ideas.OrderByDescending(x => x.Comments.Count).ToList();
                Idea CommentIdea = ideas[0];
                ViewBag.CommentIdea = CommentIdea;
                ViewBag.CommentIdeaCount = CommentIdea.Comments.Count;
                //most view
                foreach (Idea idea in ideas)
                {
                    List<View> views = db.Views.ToList();
                    idea.Views = views.Where(x => x.Idea == idea).ToList();

                }
                ideas = ideas.OrderByDescending(x => x.Views.Count).ToList();
                Idea ViewIdea = ideas[0];
                ViewBag.ViewIdea = ViewIdea;
                ViewBag.ViewIdeaCount = ViewIdea.Views.Count;
                //most like
                foreach (Idea idea in ideas)
                {
                    List<Reaction> reactions = db.Reactions.ToList();
                    idea.Reactions = reactions.Where(x => x.React == true && x.Idea == idea).ToList();
                }
                ideas = ideas.OrderByDescending(x => x.Reactions.Count).ToList();

                Idea LikeIdea = ideas[0];
                ViewBag.LikeIdea = LikeIdea;
                ViewBag.LikeIdeaCount = LikeIdea.Reactions.Count;

                //total idea in submission
                int Total = ideas.Count();
                ViewBag.Total = Total;

                return View(ideas.ToPagedList(i ?? 1, 5)); 
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
          
            }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

      
    }
}
