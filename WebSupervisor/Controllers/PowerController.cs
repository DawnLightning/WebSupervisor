using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSupervisor.Models;
using System.Data;
using WebDAL;
using WebSupervisor.Code.Classes;
using PagedList;

namespace WebSupervisor.Controllers
{
    public class PowerController : Controller
    {
        List<AdminModel> adminlist = DBHelper.ExecuteList<AdminModel>("select * from [admin]", CommandType.Text, null);
        // GET: Power
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PowerManger()
        {
            return View();
        }
        public PartialViewResult Key(int page=1)
        {
            IPagedList<AdminModel> Iadmin = adminlist.ToPagedList(page, 10);
            return PartialView(Iadmin);
        }
        public ActionResult AddPower(FormCollection fc)
        {

            //SqlParameterCollection
            List<AdminModel> id = DBHelper.ExecuteList<AdminModel>("select * from admin", CommandType.Text, null);

            AdminModel model = new AdminModel();

            model.UId = id.Max(m => m.UId) + 1;
            model.UserName = fc["username"];
            model.Password = fc["password"];
            model.College = fc["college"];
            model.NumSMS = Convert.ToInt32(fc["numsms"]);
            model.Power = 0;
            model.Email = fc["email"];
            model.Phone = fc["phone"];
            string insertcommand = string.Format("SET IDENTITY_INSERT admin ON insert into admin(uid, username, password, college, numsms, power, phone, email) values({0},'{1}','{2}','{3}',{4},{5},'{6}','{7}')",
               model.UId, model.UserName, model.Password, model.College, model.NumSMS, model.Power, model.Phone, model.Email);

            if (DBHelper.ExecuteNonQuery(insertcommand, CommandType.Text, null) > 0)
            {
                DBHelper.ExecuteNonQuery("SET IDENTITY_INSERT admin OFF", CommandType.Text, null);
                return Json(new jsondata(0, "添加成功！"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new jsondata(1, "添加失败！"), JsonRequestBehavior.AllowGet);
            }


        }
    }
}
