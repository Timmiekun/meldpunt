using Meldpunt.Models;
using Meldpunt.Utils;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Meldpunt
{
  [RoutePrefix("admin/blogitems")]
  public class AdminBlogController : Controller
  {
    private MeldpuntContext db = new MeldpuntContext();

    // GET: BlogModels
    [Route]
    public ActionResult Index()
    {
      return View(db.BlogModels.ToList());
    }

   
    [Route("create")]
    public ActionResult Create()
    {
      return View();
    }

    // POST: BlogModels/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [Route("create")]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Title,MetaTitle,MetaDescription,Content,Url,UrlPart,ParentId,LastModified,Published")] BlogModel blogModel)
    {
      if (ModelState.IsValid)
      {
        blogModel.Id = Guid.NewGuid();
        db.BlogModels.Add(blogModel);
        db.SaveChanges();
        return RedirectToAction("Index");
      }

      return View(blogModel);
    }

    [Route("{id}")]
    public ActionResult Edit(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      BlogModel blogModel = db.BlogModels.Find(id);
      if (blogModel == null)
      {
        return HttpNotFound();
      }
      return View(blogModel);
    }

    // POST: BlogModels/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ValidateInput(false)]
    [Route("{id}")]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Title,MetaTitle,MetaDescription,Content,Url,UrlPart,ParentId,LastModified,Published")] BlogModel blogModel)
    {
      if (ModelState.IsValid)
      {
        blogModel.UrlPart = blogModel.UrlPart.XmlSafe();
        db.Entry(blogModel).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      return View(blogModel);
    }

    [Route("delete")]
    public ActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      BlogModel blogModel = db.BlogModels.Find(id);
      if (blogModel == null)
      {
        return HttpNotFound();
      }
      return View(blogModel);
    }

    // POST: BlogModels/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("delete")]
    public ActionResult DeleteConfirmed(Guid id)
    {
      BlogModel blogModel = db.BlogModels.Find(id);
      db.BlogModels.Remove(blogModel);
      db.SaveChanges();
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
