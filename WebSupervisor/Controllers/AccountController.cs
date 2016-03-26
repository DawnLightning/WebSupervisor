using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSupervisor.Models;
using WebDAL;
using System.Data;
using System.Data.SqlClient;
using WebSupervisor.Code.Placement;
using WebSupervisor.Code.Classes;

namespace WebSupervisor.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
           
            return View();
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
           
            string username = fc["username"];
            string password = fc["password"];
            //List<SupervisorModel> lstsupervisor = new List<SupervisorModel>();
            List<AdminModel> lstadmin = new List<AdminModel>();
            lstadmin = DBHelper.ExecuteList<AdminModel>("select * from [admin]", CommandType.Text, null);
            //lstsupervisor = DBHelper.ExecuteList<SupervisorModel>("select phone,password from [teachers] where password is not null", CommandType.Text, null);
            foreach (AdminModel admin in lstadmin)
            {
                if (admin.Password == password && admin.UserName == username)
                {
                    if (admin.Power == 0)
                    {
                        Session["College"] = admin.College;
                        Session["AdminUser"] = username;
                        Session["Power"] = "管理员";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (admin.Power == 1)
                    {
                        Session["AdminUser"] = username;
                        Session["Power"] = "超级管理员";
                        return RedirectToAction("Index", "Home");
                    }
                        
                }
                    
            }
            //foreach (SupervisorModel supervisor in lstsupervisor)
            //{
            //    if (supervisor.Phone == username && supervisor.Password == password)
            //    {
            //        Session["AdminUser"] = username;
            //        return RedirectToAction("CheifSupervisor", "Supervisor", "");
            //    }

            //}
            return this.Json(new jsondata(0, "账号或密码有错，请核对！"), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            if (Session["AdminUser"] != null)
            {
                Session["AdminUser"] = null;
            }
            return RedirectToAction("Login");
        }
    }
}
