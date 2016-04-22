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
            return this.Json(new mkjson("账号或密码有错，请核对！", 1), JsonRequestBehavior.AllowGet);
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
        public PartialViewResult TeacherInfo()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult InfoTeacher(TeachersModel te)
        {
            try
            {
                te.Tid = collegeid(te.College) + te.TeacherName;
                string insertteacher = string.Format("INSERT INTO [dbo].[teachers] ([tid], [teachername], [phone], [email], [college], [indentify], [title], [islimit], [password], [teacherroom]) VALUES (N'{0}', N'{1}', N'{2}', N'{3} ', N'{4}', 0, N' {5}', 0, N'123', N'{6}')",te.Tid,te.TeacherName,te.Phone,te.Email,te.College,te.Title,te.TeacherRoom);
                DBHelper.ExecuteNonQuery(insertteacher, CommandType.Text, null);
                //DBHelper.Insert<TeachersModel>(te);
                return Json(new mkjson("添加成功！", 0));
            }
            catch (Exception) { return Json(new mkjson("添加失败！", 1)); }
        }
        private string collegeid(string college)
        {
            switch (college)
            {
                case "研究生学院":
                    return "1";
                case "第一临床医学院":
                    return "2";
                case "第二临床医学院":
                    return "3";
                case "第三临床医学院":
                    return "4";
                case "公共卫生学院":
                    return "5";
                case "护理学院":
                    return "6";
                case "基础医学院":
                    return "7";
                case "外国语学院":
                    return "8";
                case "人文与管理学院":
                    return "9";
                case "信息工程学院":
                    return "10";
                case "药学院":
                    return "11";
                case "医学检验学院":
                    return "12";
                case "继续教育学院":
                    return "13";
                case "社会科学部":
                    return "14";
                case "体育教学部":
                    return "15";
                default:
                    return "未知";

            }

        }
    }
}
