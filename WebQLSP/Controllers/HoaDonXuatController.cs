using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;
using Microsoft.EntityFrameworkCore;
using Xceed.Words.NET;
using PagedList;

namespace WebQLSP.Controllers
{
    [Authentication]
    public class HoaDonXuatController : Controller
    {
        // GET: HoaDonXuat
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
                var list = db.InvoiceOuts.Where(x => x.Inv_ID.ToUpper().Contains(SearchString.ToUpper()) && x.isDelete == false).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var list = db.InvoiceOuts.Where(x => x.isDelete == false).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }

        }

        public ActionResult Create()
        {
            InvoiceOut inv = new InvoiceOut();
            inv.DetailInvoiceOuts.Add(new DetailInvoiceOut() { ID = 1 });
            return View(inv);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvoiceOut invoice, string phone)
        {
            var cus = db.Customers.SingleOrDefault(x => x.Cus_Phone == phone);
            if (ModelState.IsValid)
            {
                var emp = (int)Session["Emp_Id"];
                invoice.Emp_ID = emp;
                invoice.Cus_ID = cus.Cus_ID;
                db.InvoiceOuts.Add(invoice);

                foreach (var detail in invoice.DetailInvoiceOuts)
                {
                    Product prod = db.Products.SingleOrDefault(p => p.Prod_ID == detail.Prod_ID);
                    if (prod != null)
                    {
                        prod.Quantity -= detail.Quantity;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Trả về view với các thông báo lỗi
                return View(invoice);
            }
        }

        public ActionResult Edit(string id)
        {
            InvoiceOut inv = db.InvoiceOuts.SingleOrDefault(i => i.Inv_ID == id);
            if (inv == null)
            {
                return HttpNotFound();
            }
            inv.DetailInvoiceOuts = db.DetailInvoiceOuts.Where(d => d.Inv_ID == id).ToList();
            return View(inv);
        }

        [HttpPost]
        public ActionResult Edit(InvoiceOut model)
        {
          
            InvoiceOut inv = db.InvoiceOuts.SingleOrDefault(i => i.Inv_ID == model.Inv_ID);
            if (inv == null)
            {
                return HttpNotFound();
            }
            inv.Inv_DateOut = model.Inv_DateOut;

            for (int i = 0; i < inv.DetailInvoiceOuts.Count; i++)
            {
                inv.DetailInvoiceOuts[i].Prod_ID = model.DetailInvoiceOuts[i].Prod_ID;
                inv.DetailInvoiceOuts[i].Quantity = model.DetailInvoiceOuts[i].Quantity;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            InvoiceOut inv = db.InvoiceOuts.SingleOrDefault(x => x.Inv_ID == id);
            if (inv == null)
            {
                return HttpNotFound();
            }
            var detail = db.DetailInvoiceOuts.Where(d => d.Inv_ID == id).ToList();
            inv.DetailInvoiceOuts = detail;
            decimal total = 0;
            foreach (var item in detail)
            {
                total += item.Inv_Total;
            }

            ViewBag.Total = total.ToString("#,##0.00");
            return View(inv);
        }

        public string Delete(string id)
        {
            try
            {
                var invoice = db.InvoiceOuts.SingleOrDefault(i => i.Inv_ID == id);
                if (invoice != null)
                {
                    invoice.isDelete = true;
                    db.SaveChanges();
                    return "Xóa sản phẩm thành công";
                }
                else
                {
                    return "Không tìm thấy sản phẩm có mã số " + id;
                }
            }
            catch (Exception ex)
            {
                return "Xóa sản phẩm thất bại" + ex.Message;
            }
        }

    }
}