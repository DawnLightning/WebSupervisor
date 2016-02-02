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

            List<AdminModel> lstuser = new List<AdminModel>();
            lstuser = DBHelper.ExecuteList<AdminModel>("select * from [admin]", CommandType.Text, null);
            foreach (AdminModel user in lstuser)
            {
                if (user.Password == password && user.UserName == username)
                {
                    if (user.Power == 0)
                        return RedirectToAction("Index", "Homepage", "");
                    else if (user.Power == 1)
                        return RedirectToAction("Index", "Power", "");
                }
                else
                {
                    return JavaScript("alert('用户不存在，请检查账号和密码!')");
                }
                    
            }
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
            return View();
        }
        public ActionResult Home()
        {

            return View();
        }
    }
}