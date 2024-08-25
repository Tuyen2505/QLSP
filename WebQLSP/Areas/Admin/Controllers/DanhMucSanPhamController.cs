using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;

namespace WebQLSP.Areas.Admin.Controllers
{
    public class DanhMucSanPhamController : Controller
    {

        // GET: DanhMucSanPham
        private QLSPEntities db = new QLSPEntities();
        public ActionResult Index()
        {
            var list = db.ProductCatalogs.ToList();
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductCatalog model)
        {
            if (ModelState.IsValid)
            {
                db.ProductCatalogs.Add(model);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}