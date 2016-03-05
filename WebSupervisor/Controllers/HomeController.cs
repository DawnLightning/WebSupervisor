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
using PagedList;

namespace WebSupervisor.Controllers
{   

    [AuthenAdmin]
    public class HomeController : Controller
    {
        List<TeachersModel> teacherlist = DBHelper.ExecuteList<TeachersModel>("select * from Teachers", CommandType.Text, null);
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
               
                return Content("<script language='javascript' type='text/javascript'>alert('保存成功！！');window.location.href= '/Home/Set'</script>");
            }
            catch (Exception) { return Content("<script language='javascript' type='text/javascript'>alert('保存失败！！');window.location.href= '/Home/Set'</script>"); };

        }
      
       
        public PartialViewResult Teacher(int page = 1)
        {
           
            IPagedList<TeachersModel> Iteachers = teacherlist.ToPagedList(page, 12);
            return PartialView(Iteachers);
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
      
    }
}