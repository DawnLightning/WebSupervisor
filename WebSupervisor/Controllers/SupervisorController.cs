using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using WebSupervisor.Models;
using WebDAL;
using PagedList;

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
    }
}