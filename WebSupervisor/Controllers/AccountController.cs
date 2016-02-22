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
        public ActionResult Login()
        {


            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
           
            string username = fc["username"];
            string password = fc["password"];
            List<SupervisorModle> lstsupervisor = new List<SupervisorModle>();
            List<AdminModel> lstadmin = new List<AdminModel>();
            lstadmin = DBHelper.ExecuteList<AdminModel>("select * from [admin]", CommandType.Text, null);
            lstsupervisor = DBHelper.ExecuteList<SupervisorModle>("select phone,password from [teachers] where password is not null", CommandType.Text, null);
            foreach (AdminModel admin in lstadmin)
            {
                if (admin.Password == password && admin.UserName == username)
                {
                    if (admin.Power == 0)
                    {
                        Session["College"] = admin.College;
                        Session["UserName"] = username;
                        return RedirectToAction("Index", "Home", new {role= "管理员" });
                    }
                    else if (admin.Power == 1)
                    {
                        Session["UserName"] = username;
                        return RedirectToAction("Index", "Home", new { role = "超级管理员" });
                    }
                        
                }
                    
            }
            foreach (SupervisorModle supervisor in lstsupervisor)
            {
                if (supervisor.Phone == username && supervisor.Password == password)
                {
                    Session["UserName"] = username;
                    return RedirectToAction("CheifSupervisor", "Supervisor", "");
                }
            
            }
            return Content("<script language='javascript' type='text/javascript'>alert('账号或者密码有误，请核对后再登录！！');window.location.href= '/Account/Login'</script>");
        }
    }
}
