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
        List<TeachersModel> teacherlist = DBHelper.ExecuteList<TeachersModel>("select * from teachers where indentify=1", CommandType.Text, null);
        List<SpareTimeModel> splist = DBHelper.ExecuteList<SpareTimeModel>("select * from sparetime", CommandType.Text, null);
        List<ClassesModel> classlist = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
        // GET: SupervisorPage
        public ActionResult CheifSupervisor()
        {
            return View();
        }
        public ActionResult NormalSupervisor()
        {
            return View();
        }
        public PartialViewResult Supervisor(int page = 1)
        {
            List<SupervisorViewModel> spvlist = new List<SupervisorViewModel>();
            foreach (TeachersModel teacher in teacherlist)
            {
                SupervisorViewModel m = new SupervisorViewModel();
                m.TeacherName = teacher.TeacherName;
                m.Phone = teacher.Phone;
                m.Password = teacher.Password;
                m.SpareTime = "";
                var sptlist = (from s in splist
                               where s.Tid == teacher.Tid
                               select s).ToList();
                if (sptlist.Count > 0)
                {
                    List<SpareTimeModel> sptlist1 = new List<SpareTimeModel>();
                    sptlist1 = sptlist.GroupBy(a => a.Week).Select(b => b.First()).ToList();
                    foreach (SpareTimeModel spt in sptlist1)
                    {
                        m.SpareTime = m.SpareTime + " " + spt.Week.ToString();
                    }
                }
                else
                {
                    m.SpareTime = "未填写";
                }
                spvlist.Add(m);
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
                MakeSpareTime.AutoSelectSpareTime(teacherlist[index].TeacherName);
                //string i=list[index].TeacherName;
                //string a;
            }
            return Json(new { status = 1 });
        }
        public ActionResult ArrageAddallselect(string week, string day, string classnumber, string teachername, string classtype)
        {
            int[] select = new int[] { int.Parse(week), int.Parse(day), int.Parse(classnumber) };
            List<CheckClassModel> checkclasslist = DBHelper.ExecuteList<CheckClassModel>("select * from checkclass", CommandType.Text, null);
            ArrageAddModel arrageadd = new ArrageAddModel();
            if(Session["Power"].ToString()=="管理员")
            {
                arrageadd.classeslist = (from c in classlist
                                         join t in teacherlist on c.TeacherName equals t.TeacherName
                                         where c.Week == @select[0] && c.Day == @select[1] && c.ClassNumber == @select[2] && c.TeacherName == teachername
                                         && c.ClassType == classtype&&t.College==Session["UserName"].ToString()
                                         select c).ToList();
                arrageadd.FirstSupervisorList = (from s in splist
                                                 join t in teacherlist on s.Tid equals t.Tid
                                                 where t.College == Session["UserName"].ToString()
                                                 select new FirstSupervisorModel
                                                 {
                                                     TeacherName = t.TeacherName,
                                                     IsArrage = Trueflase(s.Assign)
                                                 }).ToList();
                arrageadd.SecondSupervisorList = (from ch in checkclasslist
                                                  join t in teacherlist on ch.Tid equals t.Tid
                                                  where t.College == Session["UserName"].ToString()
                                                  select new SecondSupervisorModel
                                                  {
                                                      TeacherName = t.TeacherName,
                                                      Total = ch.total
                                                  }).ToList();
            }
            else
            {
                arrageadd.classeslist = (from c in classlist
                                         where c.Week == @select[0] && c.Day == @select[1] && c.ClassNumber == @select[2] && c.TeacherName == teachername
                                         && c.ClassType == classtype
                                         select c).ToList();
                arrageadd.FirstSupervisorList = (from s in splist
                                                 join t in teacherlist on s.Tid equals t.Tid
                                                 select new FirstSupervisorModel
                                                 {
                                                     TeacherName = t.TeacherName,
                                                     IsArrage = Trueflase(s.Assign)
                                                 }).ToList();
                arrageadd.SecondSupervisorList = (from ch in checkclasslist
                                                  join t in teacherlist on ch.Tid equals t.Tid
                                                  select new SecondSupervisorModel
                                                  {
                                                      TeacherName = t.TeacherName,
                                                      Total = ch.total
                                                  }).ToList();
            }
            return Json(arrageadd);
        }
        private string Trueflase(int i)
        {
            string chiness_tureorflase;
            if (i==1)
            {
                chiness_tureorflase = "已安排";
                return chiness_tureorflase;
            }
            else
            {
                chiness_tureorflase = "未安排";
                return chiness_tureorflase;
            }

        }
    }
}