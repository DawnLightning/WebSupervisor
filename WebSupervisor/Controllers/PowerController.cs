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
using System.IO;
using System.Text;
using WebSupervisor.Controllers.CheckUser;

namespace WebSupervisor.Controllers
{
    [AuthenAdmin]
    public class PowerController : Controller
    {
        List<AdminModel> adminlist = DBHelper.ExecuteList<AdminModel>("select * from admin", CommandType.Text, null);
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
            //List<AdminModel> adminlist = DBHelper.ExecuteList<AdminModel>("select * from admin", CommandType.Text, null);

            AdminModel model = new AdminModel();

            model.UId = adminlist.Max(m => m.UId) + 1;
            model.UserName = fc["username"];
            model.Password = fc["password"];
            model.College = fc["college"];
            //model.NumSMS = Convert.ToInt32(fc["numsms"]);
            model.Power = 0;
            model.Email = fc["email"];
            model.Phone = fc["phone"];
            string insertcommand = string.Format("SET IDENTITY_INSERT admin ON insert into admin(uid, username, password, college,power, phone, email) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
               model.UId, model.UserName, model.Password, model.College,model.Power, model.Phone, model.Email);

            if (DBHelper.ExecuteNonQuery(insertcommand, CommandType.Text, null) > 0)
            {
                DBHelper.ExecuteNonQuery("SET IDENTITY_INSERT admin OFF", CommandType.Text, null);
                //return Json(new jsondata(0, "添加成功！"), JsonRequestBehavior.AllowGet);
                return Redirect("/#!/Power/Key");
            }
            else
            {
                return Json(new mkjson("添加失败！", 1), JsonRequestBehavior.AllowGet);
            }


        }
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAdmin()
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
                    string delete_admin = string.Format("delete from admin where uid={0}", idarray[i]);
                    DBHelper.ExecuteNonQuery(delete_admin, CommandType.Text, null);
                }

                return this.Json(new mkjson("删除成功", 0), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return this.Json(new mkjson("删除失败", 1), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateAdmin(string uid,string property,string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value.Trim()))
                {
                    string updateteachers = string.Format("update admin set {0}='{1}' where uid='{2}'", property, value, uid);
                    DBHelper.ExecuteNonQuery(updateteachers, CommandType.Text, null);
                }
                return Json(new mkjson("更新成功", 0));
            }
            catch (Exception) { return Json(new mkjson("更新失败", 1)); }
        }
    }
}
