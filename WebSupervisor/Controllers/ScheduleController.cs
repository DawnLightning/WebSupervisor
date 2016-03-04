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

namespace WebSupervisor.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public PartialViewResult Schedule(int pageno=1)
        {
            string path = Server.MapPath(Common.ConfPath);
            ViewBag.path = path;
            //DBHelper db = new DBHelper();
            List<ClassesModel> lstclasses = new List<ClassesModel>();
            lstclasses = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
                //db.GetCurrentData(DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null), pageno, 14);
            IPagedList<ClassesModel>  Lclasses = lstclasses.ToPagedList(pageno, 14);
            return PartialView(Lclasses);
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
            //文件存放路径格式：~/App_Data/用户名/文件名
            string virtualPath = string.Format("~/App_Data/{0}/{1}", Session["AdminUser"], filename);

            ////例如：/files/upload/20130913/43CA215D947F8C1F1DDFCED383C4D706.jpg
            //string fileMD5 = CommonFuncs.GetStreamMD5(Filedata.InputStream);
            //string FileEextension = Path.GetExtension(Filedata.FileName);
            //string uploadDate = DateTime.Now.ToString("yyyyMMdd");

            //string imgType = Request["imgType"];
            //string virtualPath = "/";
            //if (imgType == "normal")
            //{
            //    virtualPath = string.Format("~/files/upload/{0}/{1}{2}", uploadDate, fileMD5, FileEextension);
            //}
            //else
            //{
            //    virtualPath = string.Format("~/files/upload2/{0}/{1}{2}", uploadDate, fileMD5, FileEextension);
            //}
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
    }
}
