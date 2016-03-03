using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSupervisor.Models;
using System.Data.SqlClient;
using System.Data;
using WebDAL;
using WebSupervisor.Code.Classes;
using WebSupervisor.Controllers.CheckUser;

namespace WebSupervisor.Controllers
{
    [AuthenAdmin]
    public class HomeController : Controller
    {
        // GET: HomePage
        public ActionResult Index()
        {
            ViewBag.role = Session["Power"];
            return View();
        }
        public PartialViewResult Confirm()
        {

            return PartialView();
        }
        public PartialViewResult Set()
        {
            return PartialView();
        }
        public PartialViewResult Teacher()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddTeacher(FormCollection fc)
        {

            int a;
            string ischeck = fc["checkall"];
            if (ischeck == "on")
                a = 1;
            else a = 0;
            //SqlParameterCollection
                 TeachersModel model = new TeachersModel();
            model.Tid = fc["teacherNO"];
            model.Title = fc["teacherTitle"];
            model.TeacherRoom = fc["teacherRoom"];
            model.TeacherName = fc["teacherName"];
            model.Phone = fc["teacherTel"];
            model.Password = "123";
            model.Islimit =1;
            model.Indentify = a;
            model.Email = fc["teacherEmail"];
            model.College = fc["college"];


                DBHelper.Insert<TeachersModel>(model);
            return Json(new jsondata(0,"添加成功！"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Paging<T>(PageModel model,string tablename)
        {
            //model.PageSize=from s in dbo
            //Common com = new Common();
            //List<T> lst = new List<T>();
            
            int pagesum = DBHelper.ExexuteEntity<int>("select count(*) from" + tablename, CommandType.Text, null);
            string end = ((model.PageNO-1)*model.PageSize + model.PageSize).ToString();
            ////T obj = default(T);
            string selectsql = string.Format("select top {0} * from {1} where id not in (select top {2} * from {1})", end, tablename, ((model.PageNO - 1) * model.PageSize).ToString());
            List<T> lst = new List<T>();
            lst = DBHelper.ExecuteList<T>(selectsql, CommandType.Text, null);
            ////return lst;
            return Json(new { lst,model.PageNO,pagesum },JsonRequestBehavior.AllowGet);
        }
    }
}