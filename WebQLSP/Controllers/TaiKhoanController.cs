using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLSP.Models;

namespace WebQLSP.Controllers
{
    [AllowAnonymous]
    public class TaiKhoanController : Controller
    {
        QLSPEntities db = new QLSPEntities();
        // GET: TaiKhoan

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Account acc)
        {
            
            var taikhoan = acc.username;
            var matkhau = acc.pass;
            var emp = db.Accounts.SingleOrDefault(o => o.username.Equals(taikhoan) && o.pass.Equals(matkhau));

            if (emp != null)
            {
                if (ModelState.IsValid)
                {
                    Session["taikhoan"] = emp;
                    if (Session["taikhoan"] != null)
                    {
                        Session["username"] = emp.username;
                        Session["Emp_Id"] = emp.Emp_ID;
                        Session["Emp_Name"] = emp.Employee.Emp_Name;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                ViewBag.LoginFail = "Dang nhap that bai";
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Register model)
        {
            if (ModelState.IsValid)
            {

                // Tạo nhân viên và liên kết với tài khoản
                var employee = new Employee
                {
                    Emp_Name = model.Name,
                    Emp_Phone = model.PhoneNumber,
                };
                db.Employees.Add(employee);
                db.SaveChanges();
                var account = new Account
                {
                    username = model.UserName,
                    pass = model.Password,
                    Emp_ID = employee.Emp_ID,
                };
                db.Accounts.Add(account);
                db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ChangePassword(ChangePass model)
        {
            var username = Session["username"];
            var acc = db.Accounts.SingleOrDefault(x => x.username == username);
            if (ModelState.IsValid && acc != null)
            {
                if (model.OldPassword == acc.pass)
                {
                    acc.pass = model.NewPassword;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng");
                }
            }

            return View(model);
        }
    }
}