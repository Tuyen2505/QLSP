using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;
using PagedList;

namespace WebQLSP.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: Admin/KhachHang
        private QLSPEntities db = new QLSPEntities();
        public ActionResult Index(int? page)
        {
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            var list = db.Customers.ToList();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Customer model)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            Customer customer = db.Customers.SingleOrDefault(i => i.Cus_ID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public ActionResult Edit(Customer model)
        {
            Customer customer = db.Customers.SingleOrDefault(i => i.Cus_ID == model.Cus_ID);
            if (customer != null)
            {
                customer.Cus_Name = model.Cus_Name;
                customer.Cus_Phone = model.Cus_Phone;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }
    }
}