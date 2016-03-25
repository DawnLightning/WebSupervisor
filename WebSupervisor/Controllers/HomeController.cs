﻿using System;
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
using WebSupervisor.Code.Placement;
using System.IO;
using System.Text;

namespace WebSupervisor.Controllers
{

    [AuthenAdmin]
    public class HomeController : Controller
    {
        List<TeachersModel> teacherlist = DBHelper.ExecuteList<TeachersModel>("select * from Teachers", CommandType.Text, null);
        List<ArrageModel> arragelist = DBHelper.ExecuteList<ArrageModel>("select * from arrage", CommandType.Text, null);
        List<ClassesModel> classlist = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
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
        public ActionResult ConfirmTemp(int page = 1, bool ajax = false)
        {
            ViewBag.path = Server.MapPath(Common.ConfPath);
            List<ConfirmModel> arragetemplist = new List<ConfirmModel>();
            if (Session["Power"].ToString() == "管理员")
            {
                arragetemplist = (from a in arragelist
                                  join b in classlist on a.Cid equals b.Cid
                                  join t in teacherlist on b.TeacherName equals t.TeacherName
                                  where a.Stauts == 0 && t.College == Session["College"].ToString()
                                  select new ConfirmModel
                                  {
                                      Cid = a.Cid,
                                      ClassName = b.ClassName,
                                      ClassContent = b.ClassContent,
                                      ClassType = b.ClassType,
                                      Major = b.Major,
                                      Address = b.Address,
                                      TeacherName = b.TeacherName,
                                      Week = b.Week,
                                      Day = b.Day,
                                      ClassNumber = b.ClassNumber,
                                      SuperVisors = a.SuperVisors
                                  }).ToList();
            }
            else
            {
                arragetemplist = (from a in arragelist
                                  join b in classlist on a.Cid equals b.Cid
                                  join t in teacherlist on b.TeacherName equals t.TeacherName
                                  where a.Stauts == 0
                                  select new ConfirmModel
                                  {
                                      Cid = a.Cid,
                                      ClassName = b.ClassName,
                                      ClassContent = b.ClassContent,
                                      ClassType = b.ClassType,
                                      Major = b.Major,
                                      Address = b.Address,
                                      TeacherName = b.TeacherName,
                                      Week = b.Week,
                                      Day = b.Day,
                                      ClassNumber = b.ClassNumber,
                                      SuperVisors = a.SuperVisors
                                  }).ToList();
            }
                IPagedList<ConfirmModel> iplarrage = arragetemplist.ToPagedList(page, 11);
            if (ajax)
                return Json(iplarrage, JsonRequestBehavior.AllowGet);
            return PartialView(iplarrage);
        }
        [HttpPost]
        public ActionResult SaveArrage(FormCollection fc)
        {
            try
            {
                var cherkbox = from x in fc.AllKeys
                               where fc[x] != "checkall"
                               select x;
                foreach (var cherkname in cherkbox)
                {
                    SqlParameter s = new SqlParameter("@cid", cherkname);
                    DBHelper.ExecuteNonQuery("update arrage set stauts=1 where cid=@cid", CommandType.Text, s);

                }
                return Json(new jsondata(1, "保存成功！"), JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new jsondata(0, "保存失败！"), JsonRequestBehavior.AllowGet); }
        }
        public ActionResult ConfirmSure(int page = 1, bool ajax = false)
        {
            List<ConfirmModel> arragetemplist = new List<ConfirmModel>();
            if (Session["Power"].ToString() == "管理员")
            {
                arragetemplist = (from a in arragelist
                                  join b in classlist on a.Cid equals b.Cid
                                  join t in teacherlist on b.TeacherName equals t.TeacherName
                                  where a.Stauts == 1 && t.College == Session["College"].ToString()
                                  select new ConfirmModel
                                  {
                                      Cid = a.Cid,
                                      ClassName = b.ClassName,
                                      ClassContent = b.ClassContent,
                                      ClassType = b.ClassType,
                                      Major = b.Major,
                                      Address = b.Address,
                                      TeacherName = b.TeacherName,
                                      Week = b.Week,
                                      Day = b.Day,
                                      ClassNumber = b.ClassNumber,
                                      SuperVisors = a.SuperVisors
                                  }).ToList();
            }
            else
            {
                arragetemplist = (from a in arragelist
                                  join b in classlist on a.Cid equals b.Cid
                                  //join t in teacherlist on b.TeacherName equals t.TeacherName
                                  where a.Stauts == 1
                                  select new ConfirmModel
                                  {
                                      Cid = a.Cid,
                                      ClassName = b.ClassName,
                                      ClassContent = b.ClassContent,
                                      ClassType = b.ClassType,
                                      Major = b.Major,
                                      Address = b.Address,
                                      TeacherName = b.TeacherName,
                                      Week = b.Week,
                                      Day = b.Day,
                                      ClassNumber = b.ClassNumber,
                                      SuperVisors = a.SuperVisors
                                  }).ToList();
            }
            ViewBag.path = Server.MapPath(Common.ConfPath);
            IPagedList<ConfirmModel> iplarrage = arragetemplist.ToPagedList(page, 11);
            if (ajax)
                return Json(iplarrage, JsonRequestBehavior.AllowGet);
            return PartialView(iplarrage);
        }
        public PartialViewResult Set()
        {
            return PartialView();
        }
        public ActionResult InputTeacher(HttpPostedFileBase Filedata)
        {
            try
            {
                // 没有文件上传，直接返回
                if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                {
                    return HttpNotFound();
                }

                //获取文件完整文件名
                string filename = Path.GetFileName(Filedata.FileName);
                //文件存放路径格式：~/App_Data/用户名/Excel/文件名
                string virtualPath = string.Format("~/App_Data/{0}/{1}/{2}", Session["AdminUser"], "教师信息表", filename);
                string fullFileName = this.Server.MapPath(virtualPath);

                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!System.IO.File.Exists(fullFileName))
                {
                    Filedata.SaveAs(fullFileName);
                    if (Session["College"] != null)
                    {
                        string s = Session["College"].ToString();
                        ExcelHelper excel = new ExcelHelper();
                        excel.ReadTeacherTable(fullFileName, Session["College"].ToString());
                    }
                }
                return Json(new jsondata(1, "成功导入"));
            }
            catch { return Json(new jsondata(0, "导入失败")); }
        }
        [HttpPost]
        public ActionResult AddTeacher(FormCollection fc)
        {

            int a;
            string ischeck = fc["checkall"];
            if (ischeck == "on")
            {
                a = 1;
            }
            else
            {
                a = 0;
            }
            //SqlParameterCollection
            TeachersModel model = new TeachersModel();
            model.Tid = fc["teacherNO"];
            model.Title = fc["teacherTitle"];
            model.TeacherRoom = fc["teacherRoom"];
            model.TeacherName = fc["teacherName"];
            model.Phone = fc["teacherTel"];
            model.Password = "123";
            model.Islimit = 1;
            model.Indentify = a;
            model.Email = fc["teacherEmail"];
            model.College = fc["college"];
            DBHelper.Insert<TeachersModel>(model);
            return Json(new jsondata(1, "添加成功！"), JsonRequestBehavior.AllowGet);
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

                return Json(new jsondata(1, "保存成功！"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new jsondata(0, "保存失败！"), JsonRequestBehavior.AllowGet);
            }

        }


        public PartialViewResult Teacher()
        {
            return PartialView();
        }
        public PartialViewResult TeacherList(int page = 1)
        {

            IPagedList<TeachersModel> Iteachers = teacherlist.ToPagedList(page, 10);
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
            catch (Exception)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('保存失败！！');window.location.href= '/Home/Set'</script>");
            }


        }
        //自动生成安排
        [HttpPost]
        public ActionResult AutoArrange(ArrageConfigModel ac)
        {
            MakePlacement mp = new MakePlacement(ac);
            mp.CreatPlan(Session["College"].ToString());
            return Redirect("/#!/Home/Confirm");
        }
        //根据周天节次获得教师姓名
        public ActionResult ArrageAddwdc(string week, string day, string classnumber)
        {
            int[] select = new int[] { int.Parse(week), int.Parse(day), int.Parse(classnumber) };
            List<string> teachernames = new List<string>();
            if (Session["Power"].ToString() == "管理员")
            {
                teachernames = (from t in teacherlist
                                join c in classlist on t.TeacherName equals c.TeacherName
                                where t.College == Session["College"].ToString()
                                && c.Week == @select[0] && c.Day == @select[1] && c.ClassNumber == @select[2]
                                select t.TeacherName).ToList();
            }
            else
            {
                teachernames = (from c in classlist
                                where c.Week == @select[0] && c.Day == @select[1] && c.ClassNumber == @select[2]
                                select c.TeacherName).ToList();
            }
            return Json(teachernames);
        }
        /// <summary>
        /// 删除听课安排
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteArrage()
        {
          
            try
            {
                Stream s = System.Web.HttpContext.Current.Request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string tid = Encoding.UTF8.GetString(b);
                string result = HttpUtility.UrlDecode(tid).Replace("[", "").Replace("]", "");
                string[] ids = result.Split(',');
                string[] idarray = new string[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    idarray[i] = ids[i].Replace('"', ' ').Trim();
                }

                for (int i = 0; i < idarray.Length; i++)
                {
                    string delete_arrage = string.Format("delete from arrage where pid='{0}'", idarray[i]);
                    DBHelper.ExecuteNonQuery(delete_arrage, CommandType.Text, null);
                }

                return this.Json(new jsondata(0, "删除成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return this.Json(new jsondata(1, "删除失败"), JsonRequestBehavior.AllowGet);
            }
        }
    }
}