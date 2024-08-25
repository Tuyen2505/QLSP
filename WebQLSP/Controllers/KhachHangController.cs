using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;

namespace WebQLSP.Controllers
{
    [Authentication]
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        private QLSPEntities db = new QLSPEntities();
    
        public ActionResult Index(string currentFilter, int? page, string SearchString = "")
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                var list = db.Customers.Where(x => x.Cus_Phone.ToUpper().Contains(SearchString.ToUpper())).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var list = db.Customers.ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Customer model)
        {
            if(ModelState.IsValid)
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