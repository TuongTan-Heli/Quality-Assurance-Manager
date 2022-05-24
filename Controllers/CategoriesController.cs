using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using PagedList.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
namespace WebApplication2.Controllers
{
    
    public class CategoriesController : Controller
    {
     
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index(string search, int? i)
        {
            if(User.IsInRole("Administrator")|| User.IsInRole("QA Coordinator")|| User.IsInRole("QA Manager"))
            {
                return View(db.Categories.Where(x => x.CateName.Contains(search) || search == null).ToList().ToPagedList(i ?? 1, 3));
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
           
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("QA Coordinator") || User.IsInRole("QA Manager"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
           
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Administrator") || User.IsInRole("QA Coordinator") || User.IsInRole("QA Manager"))
            {
                return View();
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
          
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CateId,CateName,CateDescription")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["AlertMessage"] = "Create categories successfully...!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("QA Coordinator") || User.IsInRole("QA Manager"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
            
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CateId,CateName,CateDescription")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Update categories successfully...!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("QA Coordinator") || User.IsInRole("QA Manager"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            else
            {
                TempData["AlertMessage"] = "Not accessible";
                return RedirectToAction("Index", "Ideas");
            }
          
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            List<Idea> ideas = db.Ideas.Include(x => x.Category).ToList();
            ideas=ideas.Where(x=>x.Category==category).ToList();
            foreach(Idea idea in ideas)
            {
                idea.Category = db.Categories.ToList()[0];
                db.Entry(idea).State = EntityState.Modified;
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["AlertMessage"] = "Delete categories successfully...!";
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
