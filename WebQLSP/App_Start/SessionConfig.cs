using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebQLSP.Models;

namespace WebQLSP.App_Start
{
    public class SessionConfig
    {
        public void SaveUser(Employee user)
        {
            HttpContext.Current.Session["user"] = user;
        }

        public Employee GetUser()
        {
            return (Employee)HttpContext.Current.Session["user"];
        }
    }
}