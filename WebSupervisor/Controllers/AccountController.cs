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
            string username = fc["账号"];
            string password = fc["密码"];
            List<SupervisorModle> lstsupervisor = new List<SupervisorModle>();
            List<AdminModel> lstadmin = new List<AdminModel>();
            lstadmin = DBHelper.ExecuteList<AdminModel>("select * from [admin]", CommandType.Text, null);
            lstsupervisor = DBHelper.ExecuteList<SupervisorModle>("select phone,password from [teachers] where password is not null", CommandType.Text, null);
            foreach (AdminModel admin in lstadmin)
            {
                if (admin.Password == password && admin.UserName == username)
                {
                    if (admin.Power == 0)
                        return RedirectToAction("Index", "HomePage", "");
                    else if (admin.Power == 1)
                        return RedirectToAction("PowerManger", "Power", "");
                }
                    
            }
            foreach (SupervisorModle supervisor in lstsupervisor)
            {
                if (supervisor.Phone == username && supervisor.Password == password)
                {
                        return RedirectToAction("CheifSupervisor", "SupervisorPage", "");
                }
            
            }
             return JavaScript("alert('用户不存在，请检查账号和密码!');");
            //user=DBHelper.ExexuteEntity<AdminModel>("select * from [admin] where username='admin'",CommandType.Text,null);
            //if (user.Password==password)
            //{
            //    return RedirectToAction("Index", "Homepage", "");
            //}
            //else
            //{
            //    return View();
            //}
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