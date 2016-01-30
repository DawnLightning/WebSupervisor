using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSupervisor.Models;
using WebDAL;
using System.Data;
using System.Data.SqlClient;

namespace WebSupervisor.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {


            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            string name = fc["账号"];
            string password = fc["密码"];
            Admin user = new Admin();
            user=DBHelper.ExexuteEntity<Admin>("select * from [admin] where username='admin'",CommandType.Text,null);
            if (user.password==password)
            {
                return RedirectToAction("Index", "Homepage", "");
            }
            else
            {
                return View();
            }
            //if (name==me&&password==pass)
            //{
            //    return RedirectToAction("Index","Homepage","");
            //}
            //else
            //{
           

            //}

        }
        public ActionResult Home()
        {

            return View();
        }
    }
}