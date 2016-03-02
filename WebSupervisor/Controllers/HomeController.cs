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
namespace WebSupervisor.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomePage
        public ActionResult Index(string role)
        {
            ViewBag.role = role;
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
            //try
            //{
            //string a = fc["teacherNo"];
            int a;
            string ischeck = fc["checkall"];
            if (ischeck == "on")
                a = 1;
            else a = 0;
                //SqlParameterCollection
                SqlParameter[] sqlpara = new SqlParameter[9];
                sqlpara[0] = new SqlParameter("@tid", fc["teacherNO"]);
                sqlpara[1] = new SqlParameter("@teachername", fc["teacherName"]);
                sqlpara[6] = new SqlParameter("@title", fc["teacherTitle"]);
                sqlpara[8] = new SqlParameter("@teacherroom", fc["teacherRoom"]);
                sqlpara[2] = new SqlParameter("@phone", fc["teacherTel"]);
                sqlpara[3] = new SqlParameter("@eamil", fc["teacherEmail"]);
                sqlpara[7] = new SqlParameter("@islimit",fc["islimit"]);
                sqlpara[4] = new SqlParameter("@college", fc["college"]);
                sqlpara[5] = new SqlParameter("@identify",a);
            DBHelper.ExecuteNonQuery("insert into teachers values(@tid,@teachername,@phone,@eamil,@college,@identify,@title,@islimit,null,@teacherroom)", CommandType.Text, sqlpara);
                return  Content("<script language='javascript' type='text/javascript'>alert('添加成功！！');window.location.href= '/Home/Teacher'</script>");
                //TeacherModel teacher = new TeacherModel();
                //teacher.Tid = fc["teacherNo"];
                //teacher
       
            //catch(Exception)
            //{
            //    return Content("<script language='javascript' type='text/javascript'>alert('添加失败！！');window.location.href= '/Home/Teacher'</script>");
            //}
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