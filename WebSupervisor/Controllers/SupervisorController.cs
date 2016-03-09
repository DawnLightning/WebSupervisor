using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using WebSupervisor.Models;
using WebDAL;
using PagedList;
using WebSupervisor.Code.Placement;

namespace WebSupervisor.Controllers
{
    public class SupervisorController : Controller
    {
        List<TeachersModel> list = DBHelper.ExecuteList<TeachersModel>("select * from teachers where indentify=1", CommandType.Text, null);
        List<SpareTimeModel> splist = DBHelper.ExecuteList<SpareTimeModel>("select * from sparetime", CommandType.Text, null);

        // GET: SupervisorPage
        public ActionResult CheifSupervisor()
        {
            return View();
        }
        public ActionResult NormalSupervisor()
        {
            return View();
        }
        public PartialViewResult Supervisor(int page=1)
        {
          
            List<SupervisorViewModel> spvlist = new List<SupervisorViewModel>();
            splist=splist.GroupBy(a => a.Week).Select(g => g.First()).ToList();
            foreach (TeachersModel teacher in list)
            {
                SupervisorViewModel m = new SupervisorViewModel();
                m.TeacherName = teacher.TeacherName;
                m.Phone = teacher.Phone;
                m.Password = teacher.Password;
                m.SpareTime = "";
                foreach (SpareTimeModel spt in splist)
                {
                    if (teacher.Tid.Equals(spt.Tid))
                    {
                        m.SpareTime = m.SpareTime +" "+spt.Week.ToString();
                    }
                    
                }
                spvlist.Add(m);
            }
           
            foreach (SupervisorViewModel sp in spvlist)
            {
                if (sp.SpareTime==""&&sp.SpareTime.Length==0)
                {
                    sp.SpareTime = "未填写";
                }
                
            }
            IPagedList<SupervisorViewModel> Iteachers = spvlist.ToPagedList(page, 10);
           
            return PartialView(Iteachers);
        }
        //自动填补空闲时间
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AutoSpare(FormCollection fc)
        {
            var cherkbox = from x in fc.AllKeys
                           //where fc[x] == "on"
                           select x;
            foreach (var cherkname in cherkbox)
            {
                int index = int.Parse(cherkname);
                MakeSpareTime.AutoSelectSpareTime(list[index].TeacherName);
                //string i=list[index].TeacherName;
                //string a;
            }
            return Json( new {status=1 } );
        }
    }
}