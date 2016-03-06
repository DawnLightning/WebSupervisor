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

namespace WebSupervisor.Controllers
{
    public class ScheduleController : Controller
    {
        //string selectcommand = "";
        List<ClassesModel> lstclasses = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
        // GET: Schedule
        public PartialViewResult Schedule(int page=1)
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
            IPagedList<ClassesModel>  Lclasses = lstclasses.ToPagedList(page, 12);
            return PartialView(Lclasses);
        }
        //[ChildActionOnly]//防止直接调用
        public ActionResult ExportClassList(string cbspcial = "全部", string cbname = "全部", string cbclass = "全部",int page=1)
        {
            var lstmodel = selectExport(cbspcial, cbname, cbclass).ToPagedList(page,10);
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
            string virtualPath = string.Format("~/App_Data/{0}/{1}/{2}", Session["AdminUser"],"Excel" ,filename);
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
        //public ActionResult ClassesPaging(PageModel model, string tablename)
        //{
        //    //model.PageSize=from s in dbo
        //    //Common com = new Common();
        //    //List<T> lst = new List<T>();

        //    int pagesum = DBHelper.ExexuteEntity<int>("select count(*) from" + tablename, CommandType.Text, null);
        //    int end = ((model.PageNO - 1) * model.PageSize + model.PageSize);//结束行数
        //    //////T obj = default(T);
        //    //string selectsql = string.Format("select top {0} * from {1} where id not in (select top {2} * from {1})", end, tablename, ((model.PageNO - 1) * model.PageSize).ToString());
        //    List<ClassesModel> lst = new List<ClassesModel>();
        //    //lst = DBHelper.ExecuteList<ClassesModel>(selectsql, CommandType.Text, null);
        //    lst = DBHelper.DataResult<ClassesModel>((model.PageNO - 1) * model.PageSize, end, tablename);
        //    ////return lst;
        //    return Json(new { lst, model.PageNO, pagesum }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult ExportCList(string cbspcial="全部", string cbname="全部", string cbclass="全部")
        {
            ExportClass ex = new ExportClass();
            string filename =cbspcial + "专业" + cbname + "老师的" + cbclass + "课程表" + ".docx";
            string virtualPath = string.Format("~/App_Data/{0}/{1}/{2}", Session["AdminUser"], "课程表", filename);
            string fullFileName = this.Server.MapPath(virtualPath);

            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            //if (!System.IO.File.Exists(Common.strAddfilesPath + "课程表"))
            //{
            //    System.IO.Directory.CreateDirectory(Common.strAddfilesPath + "课程表");
            //}
            //string filename = "课程表\\" + cbspcial + "专业" + cbname + "老师的" + cbclass + "课程表";
            //filepath = Common.strAddfilesPath + filename + ".docx";
            ex.MakeWordDoc(selectExport(cbspcial,cbname,cbclass),fullFileName);
            //MessageBox.Show("导出成功");
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
                foreach(ClassesModel c in lstclasses)
                {
                    if (c.Major == condition[2] || c.ClassName == condition[1] || c.TeacherName == condition[0])
                        clist.Add(c);
                }
            }
            return clist;
        }
    }
}
