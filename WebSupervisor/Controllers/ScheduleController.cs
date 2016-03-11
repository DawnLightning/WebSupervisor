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

namespace WebSupervisor.Controllers
{
    [AuthenAdmin]
    public class ScheduleController : Controller
    {
        //string selectcommand = "";
        List<ClassesModel> lstclasses = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
        // GET: Schedule
        public PartialViewResult Schedule()
        {
            return PartialView();
        }

        public PartialViewResult ScheduleList(int page = 1)
        {

            string path = Server.MapPath(Common.ConfPath);
            ViewBag.path = path;
            IPagedList<ClassesModel> Lclasses = lstclasses.ToPagedList(page, 12);
            return PartialView(Lclasses);
        }
        public ActionResult ScheduleInport()
        {
            return PartialView();
        }
        public ActionResult ScheduleExport(string cbspcial = "全部", string cbname = "全部", string cbclass = "全部", int page = 1)
        {

            string path = Server.MapPath(Common.ConfPath);
            ViewBag.path = path;
            string[] lstteachername = new string[lstclasses.Count];
            string[] lstclassname = new string[lstclasses.Count];
            string[] lstmajor = new string[lstclasses.Count];
            for (int i = 0; i < lstclasses.Count; i++)
            {
                lstteachername[i] = lstclasses[i].TeacherName;
                lstclassname[i] = lstclasses[i].ClassName;
                lstmajor[i] = lstclasses[i].Major;
            }
            ViewBag.TeacherName = lstteachername.Distinct().ToArray();
            ViewBag.ClassName = lstclassname.Distinct().ToArray();
            ViewBag.Major = lstmajor.Distinct().ToArray();

            var lstmodel = selectExport(cbspcial, cbname, cbclass).ToPagedList(page, 10);
            return PartialView(lstmodel);
        }
        public PartialViewResult Auto()
        {
            return PartialView();
        }
        public PartialViewResult Reference()
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
            string virtualPath = string.Format("~/App_Data/{0}/{1}/{2}", Session["AdminUser"], "Excel", filename);
            string fullFileName = this.Server.MapPath(virtualPath);

            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!System.IO.File.Exists(fullFileName))
            {
                Filedata.SaveAs(fullFileName);
                ExcelHelper excel = new ExcelHelper();
                excel.Import(fullFileName);
            }

            return this.Json(new { });
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
                Directory.CreateDirectory(path);
            ex.MakeWordDoc(selectExport(cbspcial, cbname, cbclass), fullFileName, wordpath);
            return File(fullFileName, "application/zip-x-compressed", filename);
        }
        private List<ClassesModel> selectExport(string cbspcial, string cbname, string cbclass)
        {
            //string selectcommand = "";
            List<ClassesModel> clist = new List<ClassesModel>();
            List<string> condition = new List<string>();
            if (cbname == "全部")
            {
                //condition.Add(null);
            }
            else
            {
                condition.Add(cbname);
            }
            if (cbclass == "全部")
            {
                //condition.Add(null);
            }
            else
            {
                condition.Add(cbclass);
            }
            if (cbspcial == "全部")
            {
                //condition.Add(null);
            }
            else
            {
                condition.Add(cbspcial);
            }
            if (condition.Count == 0)
                clist = lstclasses;
            else
            {
                foreach (ClassesModel c in lstclasses)
                {
                    if (c.Major == condition[2] || c.ClassName == condition[1] || c.TeacherName == condition[0])
                        clist.Add(c);
                }
            }
            return clist;
        }
    }
}
