using Meldpunt.Models;
using Meldpunt.Utils;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt
{
  [RoutePrefix("admin/blogitems")]
  public class AdminBlogController : Controller
  {
    private MeldpuntContext db;

    public AdminBlogController(MeldpuntContext _db)
    {
      db = _db;
    }

    // GET: BlogModels
    [Route]
    public ActionResult Index()
    {
      return View(db.BlogModels.ToList());
    }


    [Route("create")]
    public ActionResult Create()
    {
      return View("edit", new BlogModel());
    }

    // POST: BlogModels/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ValidateInput(false)]
    [Route("create")]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Title,MetaTitle,MetaDescription,Content,UrlPart,ParentId,Image, Intro,Published")] BlogModel blogModel)
    {
      if (ModelState.IsValid)
      {
        blogModel.Id = Guid.NewGuid();
        blogModel.UrlPart = blogModel.UrlPart.XmlSafe();
        blogModel.LastModified = DateTimeOffset.Now;
        db.Entry(blogModel).State = EntityState.Modified;
        db.BlogModels.Add(blogModel);
        db.SaveChanges();
        return RedirectToAction("Index");
      }

      return View("edit",blogModel);
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
    public ActionResult Edit([Bind(Include = "Id,Title,MetaTitle,MetaDescription,Content,UrlPart,ParentId,Image, Intro,Published")] BlogModel blogModel)
    {
      if (ModelState.IsValid)
      {
        blogModel.UrlPart = blogModel.UrlPart.XmlSafe();
        blogModel.LastModified = DateTimeOffset.Now;
        db.Entry(blogModel).State = EntityState.Modified;
        db.SaveChanges();

        UpdateRouteForBlogItem(blogModel);
        Response.RemoveOutputCacheItem("/" + blogModel.Url);
        Response.RemoveOutputCacheItem("/blog");
      }
      return View(blogModel);
    }


    [Route("delete/{id}")]
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

    private void UpdateRouteForBlogItem(BlogModel blog)
    {
      var routes = RouteTable.Routes;
      using (routes.GetWriteLock())
      {
        //get last route (default).  ** by convention, it is the standard route.
        var defaultRoute = routes.Last();
        routes.Remove(defaultRoute);

        var defaultRouteOld = routes.Last();
        routes.Remove(defaultRouteOld);

        // remove old route
        var oldRoute = routes[blog.RouteId];
        if (oldRoute != null)
          routes.Remove(oldRoute);

        //add some new route for a cms page
        routes.MapRoute(
          blog.RouteId, // Route name
          blog.Url.TrimStart('/'), // URL with parameters
          new { controller = "Blog", action = "Details", id = blog.Id } // Parameter defaults
        );

        //add back default routes
        routes.Add(defaultRouteOld);
        routes.Add(defaultRoute);
      }
    }
  }
}
