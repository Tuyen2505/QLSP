using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;
using PagedList;

namespace WebQLSP.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
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
                var list = db.Products.Where(x => x.Prod_Name.ToUpper().Contains(SearchString.ToUpper()) && x.isDelete == false).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var list = db.Products.Where(x => x.isDelete == false).ToList();
                return View(list.ToPagedList(pageNumber, pageSize));
            }

        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Create(Product model, HttpPostedFileBase fileAnh)
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
                    return RedirectToAction("Index");
                }
            }

            // Nếu ModelState không hợp lệ, quay lại view "Create" để hiển thị lỗi
            return View(model);
        }


        public ActionResult Edit(string id)
        {
            Product sp = db.Products.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }

        [HttpPost]
        public ActionResult Edit(Product model, HttpPostedFileBase fileAnh)
        {
            var sp = db.Products.Find(model.Prod_ID);

            if (sp != null)
            {
                // Update properties only if the entity is found in the database
                sp.Prod_Name = model.Prod_Name;
                sp.Prod_Price_Out = model.Prod_Price_Out;
                sp.Prod_Price_In = model.Prod_Price_In;
                sp.Quantity = model.Quantity;
                sp.Brand_ID = model.Brand_ID;
                sp.Catalog_ID = model.Catalog_ID;

                if (fileAnh != null && fileAnh.ContentLength > 0)
                {
                    // Handle file upload
                    string rootFile = Server.MapPath("/Data/");
                    string pathFile = rootFile + fileAnh.FileName;
                    fileAnh.SaveAs(pathFile);
                    sp.UrlImg = "/Data/" + fileAnh.FileName;
                }

                // Save changes to the database
                db.SaveChanges();

                // Redirect to the Index action after successful update
                return RedirectToAction("Index", "SanPham");
            }

            // Handle the case where the entity with the given ID is not found
            return HttpNotFound();
        }

        public string Delete(string id)
        {
            try
            {
                var prod = db.Products.SingleOrDefault(sp => sp.Prod_ID == id);
                if (prod != null)
                {
                    prod.isDelete = true;
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



        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
        public ActionResult ExportToExcel()
        {
            var products = db.Products.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Header row
                worksheet.Cells["A1"].Value = "Mã Sản Phẩm";
                worksheet.Cells["B1"].Value = "Product Name";
                worksheet.Cells["C1"].Value = "Giá Nhập";
                worksheet.Cells["D1"].Value = "Giá Bán";
                worksheet.Cells["E1"].Value = "Số Lượng";
                worksheet.Cells["F1"].Value = "Hãng Sản Xuất";
                worksheet.Cells["G1"].Value = "Danh Mục Sản Phẩm";



                // Style the header
                using (var range = worksheet.Cells["A1:C1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data rows
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells["A" + row].Value = product.Prod_ID;
                    worksheet.Cells["B" + row].Value = product.Prod_Name;
                    worksheet.Cells["C" + row].Value = product.Prod_Price_In;
                    worksheet.Cells["D" + row].Value = product.Prod_Price_Out;
                    worksheet.Cells["E" + row].Value = product.Quantity;
                    worksheet.Cells["F" + row].Value = product.Brand.Name;
                    worksheet.Cells["G" + row].Value = product.ProductCatalog.Name;

                    row++;
                }

                // Auto-fit columns for better appearance
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set content type and file name for the response
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Products.xlsx");

                // Write the package to the response stream
                Response.BinaryWrite(package.GetAsByteArray());
            }

            return new EmptyResult();
        }

    }
}