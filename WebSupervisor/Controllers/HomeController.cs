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
        [HttpPost]
        public ActionResult SetDate(FormCollection fc)
        {
            Common.Year = Convert.ToInt32(fc["year"]);
            Common.Month = Convert.ToInt32(fc["month"]);
            Common.Day = Convert.ToInt32(fc["day"]);
            Common com = new Common();
            string path = this.Server.MapPath(Common.ConfPath);
           com.xmlSave(path);
            try
            {
                com.xmlSave(path);
                //    string path = this.Server.MapPath(Common.ConfPath);
                //    //if (!Directory.Exists(path))//判断是否存在
                //    //{
                //    //    Directory.CreateDirectory(path);//创建新路径
                //    //}
                //    XElement xe = new XElement("Config",
                //       new XElement("Year", year.ToString()),
                //       new XElement("Month", month.ToString()),
                //       new XElement("Day", day.ToString())
                //       //new XElement("MailAddress", MailAddress),
                //       //new XElement("MailPassword", MailPassword)
                //       );
                //    if (!System.IO.File.Exists(path))
                //    {
                //        xe.Save(path);
                //    }
                //    else { return Content("<script language='javascript' type='text/javascript'>alert('保存成功！！');window.location.href= '/Home/Set'</script>"); }
                //    xe.RemoveAll();
                return Content("<script language='javascript' type='text/javascript'>alert('保存成功！！');window.location.href= '/Home/Set'</script>");
            }
            catch (Exception) { return Content("<script language='javascript' type='text/javascript'>alert('保存失败！！');window.location.href= '/Home/Set'</script>"); };

        }
        [HttpPost]
        public ActionResult SetInfo(FormCollection fc)
        {
            try
            {
                string phone = fc["phone"];
                string email = fc["email"];
                string password = fc["password"];
                string uname = string.Format("'{0}'", Session["UserName"]);
                SqlParameter[] sqlpara = new SqlParameter[3];
                sqlpara[0] = new SqlParameter("@phone", phone);
                sqlpara[1] = new SqlParameter("@email", email);
                if (password != null)
                {
                    sqlpara[2] = new SqlParameter("@password", password);
                    DBHelper.ExecuteNonQuery("update admin set phone=@phone,email=@email,password=@password where username=" + uname, CommandType.Text, sqlpara);
                }
                else
                    DBHelper.ExecuteNonQuery("update admin set phone=@phone,email=@email where username=" + uname, CommandType.Text, sqlpara);
                return Content("<script language='javascript' type='text/javascript'>alert('保存成功！！');window.location.href= '/Home/Set'</script>");
            }
            catch (Exception) { return Content("<script language='javascript' type='text/javascript'>alert('保存失败！！');window.location.href= '/Home/Set'</script>"); }
            // sqlpara[2] = new SqlParameter("@password", password);
            //DBHelper.ExecuteNonQuery("update admin set phone=@phone,email=@email,password=@password where username="+uname , CommandType.Text, sqlpara);

        }
        //public ActionResult Paging<T>(PageModel model,string tablename)
        //{
        //    //model.PageSize=from s in dbo
        //    //Common com = new Common();
        //    //List<T> lst = new List<T>();

        //    int pagesum = DBHelper.ExexuteEntity<int>("select count(*) from" + tablename, CommandType.Text, null);
        //    string end = ((model.PageNO-1)*model.PageSize + model.PageSize).ToString();
        //    ////T obj = default(T);
        //    string selectsql = string.Format("select top {0} * from {1} where id not in (select top {2} * from {1})", end, tablename, ((model.PageNO - 1) * model.PageSize).ToString());
        //    List<T> lst = new List<T>();
        //    lst = DBHelper.ExecuteList<T>(selectsql, CommandType.Text, null);
        //    ////return lst;
        //    return Json(new { lst,model.PageNO,pagesum },JsonRequestBehavior.AllowGet);
        //}
    }
}