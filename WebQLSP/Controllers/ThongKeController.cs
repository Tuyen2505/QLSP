using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebQLSP.Models;

namespace WebQLSP.Controllers
{
    [Authentication]
    public class ThongKeController : Controller
    {
        // GET: ThongKe

        private QLSPEntities db = new QLSPEntities();

        public ActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult GetStatictical()
        {
            string fromDate = Request["fromdate"];
            string toDate = Request["todate"];
            var query = from o in db.InvoiceOuts
                        join od in db.DetailInvoiceOuts
                        on o.Inv_ID equals od.Inv_ID
                        join p in db.Products
                        on od.Prod_ID equals p.Prod_ID
                        select new
                        {
                            CreateDate = o.Inv_DateOut,
                            Quantity = od.Quantity,
                            Price = p.Prod_Price_Out,
                            OrigianlPrice = p.Prod_Price_In
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreateDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreateDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OrigianlPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}