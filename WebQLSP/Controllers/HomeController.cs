using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;

namespace WebQLSP.Controllers
{
    public class HomeController : Controller
    {
        [Authentication]
        public ActionResult Index()
        {
            return View();
        }

       
    }
}