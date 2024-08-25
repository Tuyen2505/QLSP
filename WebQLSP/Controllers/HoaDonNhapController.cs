using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using PagedList;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.IO;

namespace WebQLSP.Controllers
{
    [Authentication]
    public class HoaDonNhapController : Controller
    {
        // GET: HoaDonNhap
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
                var list = db.InvoiceIns.Where(x => x.Inv_ID.ToUpper().Contains(SearchString.ToUpper()) && x.isDelete == false).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var list = db.InvoiceIns.Where(x => x.isDelete == false).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }

        }


        public ActionResult Create()
        {
            InvoiceIn inv = new InvoiceIn();
            inv.DetailInvoiceIns.Add(new DetailInvoiceIn() { ID = 1});
            return View(inv);
        }
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvoiceIn invoice)
        {
            if (ModelState.IsValid)
            {
                var emp = (int)Session["Emp_Id"];
                invoice.Emp_ID = emp;
                db.InvoiceIns.Add(invoice);

                foreach (var detail in invoice.DetailInvoiceIns)
                {
                    Product prod = db.Products.SingleOrDefault(p => p.Prod_ID == detail.Prod_ID);
                    if (prod != null)
                    {
                        prod.Quantity += detail.Quantity;
                    }

                }
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            else
            {
                return View(invoice);
            }
        }

        public ActionResult Edit(string id)
        {
            InvoiceIn inv = db.InvoiceIns.SingleOrDefault(i => i.Inv_ID == id);
            if(inv == null)
            {
                return HttpNotFound();
            }
            inv.DetailInvoiceIns = db.DetailInvoiceIns.Where(d => d.Inv_ID == id).ToList();
            return View(inv);
        }

        [HttpPost]
        public ActionResult Edit(InvoiceIn model)
        {
            InvoiceIn inv = db.InvoiceIns.SingleOrDefault(i => i.Inv_ID == model.Inv_ID);
            if (inv == null)
            {
                return HttpNotFound();
            }
            inv.Inv_DateIn = model.Inv_DateIn;

            for (int i = 0; i < inv.DetailInvoiceIns.Count; i++)
            {
                inv.DetailInvoiceIns[i].Prod_ID = model.DetailInvoiceIns[i].Prod_ID;
                inv.DetailInvoiceIns[i].Quantity = model.DetailInvoiceIns[i].Quantity;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            InvoiceIn inv = db.InvoiceIns.SingleOrDefault(x => x.Inv_ID == id);
            if (inv == null)
            {
                return HttpNotFound();
            }
            var detail = db.DetailInvoiceIns.Where(d => d.Inv_ID == id).ToList();
            inv.DetailInvoiceIns = detail;
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
                var invoice = db.InvoiceIns.SingleOrDefault(i => i.Inv_ID == id);
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
        public ActionResult AddProd()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddProd(Product model, HttpPostedFileBase fileAnh)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Prod_Name))
                {
                    ModelState.AddModelError("ProductName", "Tên sản phẩm là trường bắt buộc");
                }

                if (model.Prod_Price_In <= 0)
                {
                    ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0");
                }
                if (ModelState.IsValid)
                {
                    if (fileAnh != null && fileAnh.ContentLength > 0)
                    {
                        string rootFile = Server.MapPath("/Data/");
                        string pathFile = Path.Combine(rootFile, fileAnh.FileName);
                        fileAnh.SaveAs(pathFile);

                        model.UrlImg = "/Data/" + fileAnh.FileName;
                    }

                    db.Products.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
            }
            return View(model);
        }
    }
}