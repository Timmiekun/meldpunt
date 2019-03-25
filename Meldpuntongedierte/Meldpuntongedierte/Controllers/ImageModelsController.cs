using Meldpunt.ActionFilters;
using Meldpunt.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Meldpunt
{
  [MustBeAdmin]
  [RoutePrefix("admin/ImageModels")]
  public class ImageModelsController : Controller
  {
    private MeldpuntContext db = new MeldpuntContext();

    // GET: ImageModels
    [Route]
    public ActionResult Index()
    {
      return View(db.ImageModels.ToList());
    }

    [Route("Details/{id}")]
    public ActionResult Details(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      ImageModel imageModel = db.ImageModels.Find(id);
      if (imageModel == null)
      {
        return HttpNotFound();
      }
      return View(imageModel);
    }

    [Route("Create")]
    public ActionResult Create()
    {
      return View();
    }

    // POST: ImageModels/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Create")]
    public ActionResult Create([Bind(Include = "Id,Name")] ImageModel imageModel)
    {
      if (ModelState.IsValid)
      {
        imageModel.Id = Guid.NewGuid();
        db.ImageModels.Add(imageModel);
        db.SaveChanges();
        return RedirectToAction("Index");
      }

      return View(imageModel);
    }

    [Route("Edit/{id}")]
    public ActionResult Edit(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      ImageModel imageModel = db.ImageModels.Find(id);
      if (imageModel == null)
      {
        return HttpNotFound();
      }
      return View(imageModel);
    }

    // POST: ImageModels/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Edit/{id}")]
    public ActionResult Edit([Bind(Include = "Id,Name")] ImageModel imageModel)
    {
      if (ModelState.IsValid)
      {
        db.Entry(imageModel).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      return View(imageModel);
    }

    [Route("Delete/{id}")]
    public ActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      ImageModel imageModel = db.ImageModels.Find(id);
      if (imageModel == null)
      {
        return HttpNotFound();
      }
      return View(imageModel);
    }

    // POST: ImageModels/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("Delete/{id}")]
    public ActionResult DeleteConfirmed(Guid id)
    {
      ImageModel imageModel = db.ImageModels.Find(id);
      db.ImageModels.Remove(imageModel);
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
