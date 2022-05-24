using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class SubmissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Submissions
        public ActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return View(db.Submissions.ToList());
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
            
        }

        // GET: Submissions/Details/5
        public ActionResult Details(int? id)
        {
            if (User.IsInRole("Administrator"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Submission submission = db.Submissions.Find(id);
                if (submission == null)
                {
                    return HttpNotFound();
                }
                return View(submission);
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
           
        }

        // GET: Submissions/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Administrator"))
            {
                return View();
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
            
        }

        // POST: Submissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Submission submission,DateTime OpenDate,DateTime CloseDate,DateTime FinalCloseDate)
        {
            
            if (ModelState.IsValid)
            {
                submission.Open_date = OpenDate;
                submission.Closure_date = CloseDate;
                submission.Final_closure_date = FinalCloseDate;
                db.Submissions.Add(submission);
                db.SaveChanges();
                TempData["AlertMessage"] = "Create new submissions successfully...!";

                return RedirectToAction("Index");
            }

            return View(submission);
        }

        // GET: Submissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("Administrator"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Submission submission = db.Submissions.Find(id);
                if (submission == null)
                {
                    return HttpNotFound();
                }
                return View(submission);
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
          
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubId,SubName,SubDescription,Closure_date,Final_closure_date")] Submission submission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(submission).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Update submissions successfully...!";

                return RedirectToAction("Index");
            }
            return View(submission);
        }

        // GET: Submissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (User.IsInRole("Administrator"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Submission submission = db.Submissions.Find(id);
                if (submission == null)
                {
                    return HttpNotFound();
                }
                return View(submission);
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
          
        }

        // POST: Submissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Submission submission = db.Submissions.Find(id);
            
            List<Idea> ideas = db.Ideas.ToList();
            ideas = ideas.Where(idea => idea.Submission == submission).ToList();
            Submission submission1 = db.Submissions.Where(x=>x.Open_date<=DateTime.Now&&DateTime.Now<=x.Closure_date).FirstOrDefault();
            foreach(Idea idea in ideas)
            {

                idea.Submission = submission1;
                db.Entry(idea).State = EntityState.Modified;

            }
            db.Submissions.Remove(submission);
            db.SaveChanges();
            TempData["AlertMessage"] = "Delete submissions successfully...!";

            return RedirectToAction("Index");
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
