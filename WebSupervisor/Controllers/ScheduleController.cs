using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDAL;
using WebSupervisor.Models;
using PagedList;
using PagedList.Mvc;
using WebSupervisor.Code.Classes;
using WebSupervisor.Code.Word;
using WebSupervisor.Controllers.CheckUser;
using System.Text;

namespace WebSupervisor.Controllers
{
    [AuthenAdmin]
    public class ScheduleController : Controller
    {
        
        List<ClassesModel> lstclasses = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
        static List<ReportFileStatusModel>  lstfile = new List<ReportFileStatusModel>();
        // GET: Schedule+
        public PartialViewResult Schedule()
        {
            return PartialView();
        }

        public PartialViewResult ScheduleList(int page = 1)
        {
            string path = Server.MapPath(Common.ConfPath);
            ViewBag.path = path;
            if (Session["Power"].ToString() == "管理员")
            {
                List<TeachersModel> techernames = DBHelper.ExecuteList<TeachersModel>("SELECT teachername FROM [dbo].[teachers] where college='" + Session["College"].ToString()+"'", CommandType.Text, null);
                IPagedList<ClassesModel> Lclasses = (from c in lstclasses
                                                     join tn in techernames on c.TeacherName equals tn.TeacherName
                                                     select c).ToPagedList(page, 12);
                return PartialView(Lclasses);
            }
            else
            {
                IPagedList<ClassesModel> Lclasses = lstclasses.ToPagedList(page, 12);
                return PartialView(Lclasses);
            }

        }
    
        public ActionResult ScheduleExport(string cbspcial = "全部", string cbname = "全部", string cbclass = "全部",int page = 1)
        {

            string path = Server.MapPath(Common.ConfPath);
            ViewBag.path = path;
            List<TeachersModel> teacherlist = new List<TeachersModel>();
            //var teacherlist=DBHelper.ExecuteList
            if (Session["College"] != null)
                teacherlist = DBHelper.ExecuteList<TeachersModel>("SELECT * FROM [dbo].[teachers] where college='" + Session["College"].ToString() + "'", CommandType.Text, null);
            else teacherlist = DBHelper.ExecuteList<TeachersModel>("SELECT * FROM [dbo].[teachers]", CommandType.Text, null);
            var classesl = (from c in lstclasses
                           join t in teacherlist on c.TeacherName equals t.TeacherName
                           select c).ToList();
            var lstmodel = selectExport(cbspcial, cbname, cbclass, classesl);
            ViewBag.TeacherName = lstmodel.Select(t=>t.TeacherName).Distinct().ToArray();
            ViewBag.ClassName = lstmodel.Select(c=>c.ClassName).Distinct().ToArray();
            ViewBag.Major = lstmodel.Select(m=>m.Major).Distinct().ToArray();
            return PartialView(lstmodel.ToPagedList(page, 10));
        }
        public PartialViewResult Auto()
        {
            return PartialView();
        }
        public ActionResult Upload(HttpPostedFileBase Filedata)
        {


            // 没有文件上传，直接返回
            if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
            {
                return HttpNotFound();
            }

            //获取文件完整文件名
            string filename = Path.GetFileName(Filedata.FileName);
            //文件存放路径格式：~/App_Data/用户名/Excel/文件名
            string virtualPath = string.Format("~/App_Data/{0}/{1}/{2}", Session["AdminUser"], "进度表", filename);
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


            }
            else
            {
                System.IO.File.Delete(fullFileName);
                Filedata.SaveAs(fullFileName);
            }
            if (Session["College"] != null)
            {
                ExcelHelper excel = new ExcelHelper();

                int code = excel.Import(fullFileName, Session["College"].ToString());
                switch (code)
                {
                    case 0:
                        return Json(new mkjson(filename, 1));
                    case 1:
                        return Json(new mkjson(filename, 0));
                    case -1:
                        return Json(new mkjson(filename, 1));
                    default:
                        return Json(new mkjson(filename, 1));
                }
            }
            else return Json(new mkjson(filename, 0));

        }
        public ActionResult ExportCList(string cbspcial = "全部", string cbname = "全部", string cbclass = "全部")
        {
            ExportClass ex = new ExportClass();
            string filename = cbspcial + "专业" + cbname + "老师的" + cbclass + "课程表" + ".docx";
            string virtualPath = string.Format("~/App_Data/{0}/{1}/{2}", Session["AdminUser"], "课程表", filename);
            string fullFileName = this.Server.MapPath(virtualPath);
            string wordpath = Server.MapPath("~/Resources/classes.docx");
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
              
            ex.MakeWordDoc(selectExport(cbspcial, cbname, cbclass,lstclasses), fullFileName, wordpath);
            return File(fullFileName, "application/zip-x-compressed", filename);
        }
      
        [HttpPost]
        public ActionResult ScheduleInport()
        {
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string json = Encoding.UTF8.GetString(b);
            ReportFileStatusModel f = Common.JsonToObject<ReportFileStatusModel>(json);
            lstfile.Add(f);
            IPagedList<ReportFileStatusModel> Lfile = lstfile.ToPagedList(1, 12);
            return PartialView("ScheduleInport", Lfile);
        }
        [HttpGet]
        public ActionResult ScheduleInport(int page = 1)
        {
           
            IPagedList<ReportFileStatusModel> Lfile = lstfile.ToPagedList(page, 12);
            return PartialView(Lfile);
        }
        private List<ClassesModel> selectExport(string cbspcial, string cbname, string cbclass,List<ClassesModel> l)
        {
            List<ClassesModel> clist = new List<ClassesModel>();
            if (cbname == "全部" && cbclass == "全部" && cbspcial == "全部")
                clist = l;
            else
            {
                foreach (ClassesModel c in l)
                {

                    if (c.Major.Contains( cbspcial) || c.ClassName.Contains( cbclass) || c.TeacherName == cbname)
                    {
                        clist.Add(c);
                    }

                }

            }
            //var clis = (from c in l
            //            where c.Major.Contains("临床医学")
            //            select c).ToList();
            return clist;
        }
    }
}
