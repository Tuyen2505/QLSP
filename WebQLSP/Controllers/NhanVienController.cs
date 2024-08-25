using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;

namespace WebQLSP.Controllers
{
    [Authentication]
    public class NhanVienController : Controller
    {
        private QLSPEntities db = new QLSPEntities();
        public ActionResult Index()
        {
            var list = db.Employees.ToList();
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Create(Employee model)
        {
            db.Employees.Add(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            var list = db.Employees.SingleOrDefault(emp => emp.Emp_ID == id);
            return View(list);
        }
        [HttpPost]
        public ActionResult Edit(Employee model)
        {
            var emp = db.Employees.SingleOrDefault(e => e.Emp_ID == model.Emp_ID);
            if(emp != null)
            {
                emp.Emp_Name = model.Emp_Name;
                emp.Emp_Phone = model.Emp_Phone;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}

