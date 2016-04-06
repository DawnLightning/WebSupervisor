using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDAL;
using WebSupervisor.Models;
using System.Data;
namespace WebSupervisor.Code.Placement
{
    public class MakePlacement
    {
        static List<int> spareclass = new List<int> { 12, 13, 23, 24, 34, 35, 45, 67, 68, 78, 79, 89, 1011, 1112, 1012 };//枚举所有的连续节次
        List<ClassesModel> listclasses = new List<ClassesModel>();
        List<TeachersModel> listsupervisor = new List<TeachersModel>();
        List<CheckClassModel> listcheckcount = new List<CheckClassModel>();
        List<SpareTimeModel> listsparetime = new List<SpareTimeModel>();
        List<ArrageModel> listarrage = new List<ArrageModel>();//安排表
        List<SpareTimeModel> temp = new List<SpareTimeModel>();//用于暂存sparetime的数据，提高查询效率
        List<SpareTimeModel> listchange = new List<SpareTimeModel>();//存储更新过的数据,避免过多的数据更新
        List<ClassesModel> listmodfiyclass = new List<ClassesModel>();//存储更新过的数据,避免过多的数据更新
        private int Week;//周数
        private int Day;//上课天
        private int Index = 0;//数组spareclass的索引
        private ArrageConfigModel config = null;
        public MakePlacement(ArrageConfigModel config)
        {
            this.config = config;
           
        }
        /// <summary>
        /// 筛选数据
        /// </summary>
        /// <param name="college">学院</param>
        /// <returns>0代表失败，1代表成功</returns>
        private int initdata(string college)
        {
            try
            {
                string select_teacher = string.Format("select * from teachers where college='{0}'", college);
                listsupervisor = DBHelper.ExecuteList<TeachersModel>(select_teacher, CommandType.Text, null);//督导表
                if (listsupervisor.Count >= 0)
                {
                    foreach (TeachersModel tmodel in listsupervisor)
                    {
                        if (tmodel.Indentify==1)
                        {
                            string select_sparetime = string.Format("select * from sparetime where tid='{0}'", tmodel.Tid);
                            string select_checkclass = string.Format("select * from checkclass where tid='{0}'", tmodel.Tid);

                            List<SpareTimeModel> sptemp = DBHelper.ExecuteList<SpareTimeModel>(select_sparetime, CommandType.Text, null);
                            List<CheckClassModel> chtemp = DBHelper.ExecuteList<CheckClassModel>(select_checkclass, CommandType.Text, null);

                            CopyDataToList<SpareTimeModel>(sptemp, listsparetime);
                            CopyDataToList<CheckClassModel>(chtemp, listcheckcount);
                        }
                        string select_class = string.Format("select * from classes where teachername like '%{0}%'", tmodel.TeacherName);
                        List<ClassesModel> classestemp = DBHelper.ExecuteList<ClassesModel>(select_class, CommandType.Text, null);
                        CopyDataToList<ClassesModel>(classestemp, listclasses);

                    }
                    return 1;
                   
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception )
            {
                return 0;
            }



        }
        /// <summary>
        /// 将临时筛选的数据放入全局变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">临时数据</param>
        /// <param name="target">保存变量</param>
        private void CopyDataToList<T>(List<T> source,List<T> target)
        {
            foreach (T t in source)
            {
                target.Add(t);
            }
        }
        /// <summary>
        /// 重新生成听课安排
        /// </summary>
        public void ReCreatPlan(string college)
        {
            if (ResetArrageData()>0)
            {
                ResetCheckClass();
                ResetClases();
                ResetSpareTime();
                CreatPlan(college);
            }
        }
        /// <summary>
        /// 生成听课安排
        /// </summary>
        public void CreatPlan(string college)
        {
            if (config!=null&& initdata(college)>0)
            {
                for (Week = config.Bweek; Week < config.Eweek; Week++)
                {
                    UpdataWeek(listcheckcount);
                    if (GetWeekSpareCount(Week)>=config.MinPeople)
                    {
                        for (Day=1;Day<6;Day++)
                        {
                            if (GetDaySpareCount(Day) >= config.MinPeople && GetWeekArrageCount(Week) <= config.PlanNumber - 1)
                            {
                                UpdataDay(listcheckcount);                                                                                                                          
                                    for (Index = 0; Index < 15; Index++)
                                    {
                                      
                                        ClassesModel classmodel = GetClass(Week, Day, spareclass[Index]);
                                    //做非空判断，如果为空就不满足听课条件了（没上课老师）,循环继续
                                    if (classmodel != null)
                                    {
                                        List<SpareTimeModel> sptlist = GetSpareTimeList(spareclass[Index]);
                                        string group = "";//督导小组
                                        int count = sptlist.Count;
                                        if (count < config.MinPeople)
                                        {
                                            break;
                                        }
                                        else if (count >= config.MinPeople && count <= config.MaxPeople)
                                        {
                                            foreach (SpareTimeModel spt in sptlist)
                                            {
                                                group = group + "," + IdToName(spt.Tid);
                                               
                                            }
                                            WritePlacement(sptlist, classmodel, group);
                                        }
                                        else if (count > config.MaxPeople)
                                        {
                                            Random r = new Random();
                                            int numpeople = r.Next(config.MinPeople,config.MinPeople);
                                            for (int i = 0; i <numpeople; i++)
                                            {
                                                group = group + "," + IdToName(sptlist[i].Tid);
                                              
                                            }
                                            WritePlacement(sptlist, classmodel, group);
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    }
                                }
                                
                            }
                        }
                    }
                }
                UpdateAll();
            }
            

        } 
        /// <summary>
        /// 所有督导的周听课归零
        /// </summary>
        /// <param name="list"></param>
        private void UpdataWeek(List<CheckClassModel> list)
        {
            temp.Clear();//清空临时数据
            foreach (CheckClassModel model in list)
            {
                model.WeekNumber = 0;
            }
        }
        /// <summary>
        /// 所有督导的日听课归零
        /// </summary>
        /// <param name="list"></param>
        private void UpdataDay(List<CheckClassModel> list)
        {
            foreach (CheckClassModel model in list)
            {
                model.DayNumber = 0;
            }
        }
        /// <summary>
        /// 更新周听课，日听课，总听课
        /// </summary>
        /// <param name="tid"></param>
        private void UpdateCheckCount(string tid)
        {
            
                foreach (CheckClassModel cmodel in listcheckcount)
                {
                    if (cmodel.Tid.Equals(tid))
                    {
                        cmodel.WeekNumber++;
                        cmodel.DayNumber++;
                        cmodel.total++;
                        
                    }
                }
            
        }
        /// <summary>
        /// 提交所有数据
        /// </summary>
        private void UpdateAll()
        {
            foreach (ClassesModel model in listmodfiyclass)
            {
                DBHelper.Update<ClassesModel>(model);
            }
            foreach (SpareTimeModel spt in listchange)
            {

                string update = string.Format("update sparetime set tid={0},week={1},day={2},classnumber={3},assign={4} where tid={5} and week={6} and day={7} and classnumber={8}",
                    spt.Tid,spt.Week,spt.Day,spt.ClassNumber,spt.Assign, spt.Tid, spt.Week, spt.Day, spt.ClassNumber);
                DBHelper.ExecuteNonQuery(update,CommandType.Text,null);
            }
            foreach (CheckClassModel cmodel in listcheckcount)
            {
                DBHelper.Update<CheckClassModel>(cmodel);
            }
            foreach (ArrageModel arrage in listarrage)
            {
                DBHelper.Insert<ArrageModel>(arrage);
            }
        }
        /// <summary>
        /// 查询当前周次的听课安排
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        private int GetWeekArrageCount(int week)
        {
            int count = 0;
            foreach (ArrageModel arragemodel in listarrage)
            {
                foreach (ClassesModel classmodel in listclasses)
                {
                    if (arragemodel.Cid.Equals(classmodel.Cid)&&classmodel.Week.Equals(week))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        /// <summary>
        /// 查询指定周次的督导人数
        /// </summary>
        /// <param name="Week"></param>
        /// <returns></returns>
        private int GetWeekSpareCount(int week)
        {
            int count = 0;
            foreach (SpareTimeModel model in listsparetime)
            {
                if (model.Week.Equals(week))
                {
                    count++;
                    temp.Add(model);
                }
            }
            return count;
        }
        /// <summary>
        /// 查询指定天的督导人数
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private int GetDaySpareCount(int day)
        {
            int count = 0;
            List<SpareTimeModel>  secondtemp = new List<SpareTimeModel>();
            foreach (SpareTimeModel model in temp)
            {
                if (model.Day.Equals(day))
                {
                    count++;
                    secondtemp.Add(model);
                }
            }
            temp.Clear();
            foreach (SpareTimeModel model in secondtemp)
            {
                temp.Add(model);
            }
            return count;
        }
        /// <summary>
        /// 获取周天节次对应的空闲信息
        /// </summary>
        /// <param name="classnumber"></param>
        /// <returns></returns>
        private List<SpareTimeModel> GetSpareTimeList(int classnumber)
        {
            List<SpareTimeModel> list = new List<SpareTimeModel>();
            List<CheckClassModel> truelist = new List<CheckClassModel>();
            listcheckcount=listcheckcount.OrderBy(m => m.total).ToList();
            foreach (CheckClassModel model in listcheckcount)
            {
                if (model.DayNumber==config.DayListen&&model.WeekNumber<config.WeekListen)
                {
                    truelist.Add(model);
                }
            }
            foreach (CheckClassModel cmodel in truelist)
            {
                foreach (SpareTimeModel model in temp)
                {
                    if (model.ClassNumber.Equals(classnumber)&&cmodel.Tid.Equals(model.Tid))
                    {
                        list.Add(model);
                        break;
                    }
                }
            }
            

            return list;
        }
        /// <summary>
        /// 选择周天节次对应的课程，升序排列选第一个门
        /// </summary>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="classnumber"></param>
        /// <returns></returns>
        private ClassesModel GetClass(int week,int day ,int classnumber)
        {
          
            List<ClassesModel> list = new List<ClassesModel>();
            string classtype = ContorlProportion();//控制理论和实验课的比例
            foreach (ClassesModel model in listclasses)
            {
                if (model.Week.Equals(week)&&model.Day.Equals(day)&&model.ClassNumber.Equals(classnumber)&&model.ClassType.Equals(classtype))
                {
                    list.Add(model);
                }
            }
            list = list.OrderBy(m => m.CheckNumber).ToList();
            //做数量判断，如果为0表示没有上课老师,因为上面做了比例控制，可能会没有实验或者理论课
            if (list.Count>0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
           

        }
        /// <summary>
        /// 控制理论课和实验课的比例
        /// </summary>
        /// <returns></returns>
        private string ContorlProportion()
        {
            Random rd = new Random();
            int i = rd.Next(1, 100);
            string classtype = "";
            if (i > 0 && i <=config.Apercent)
            {

                classtype = "理论";
            }
            else
            {
                classtype = "实验";
            }
            return classtype;
        }
        /// <summary>
        /// 将教师id转换为名字
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        private string IdToName(string tid)
        {
            foreach (TeachersModel model in listsupervisor)
            {
                if (tid.Equals(model.Tid))
                {
                    return model.TeacherName;
                }
            }
            return tid;
        }
        /// <summary>
        /// 生成听课安排
        /// </summary>
        /// <param name="sptlist"></param>
        /// <param name="model"></param>
        /// <param name="group"></param>
        private void WritePlacement(List<SpareTimeModel> sptlist,ClassesModel model,string group)
        {
            ArrageModel arragemodel = new ArrageModel();
            arragemodel.Cid = model.Cid;
            arragemodel.SuperVisors = group.Substring(1);
            arragemodel.Stauts = 0;
            arragemodel.Pid = model.Cid.Trim() + sptlist[0].Week + sptlist[0].Day + sptlist[0].ClassNumber.ToString();
            listarrage.Add(arragemodel);
          
            model.CheckNumber++;
            listmodfiyclass.Add(model);
            foreach (SpareTimeModel spt in sptlist)
            {
                UpdateCheckCount(spt.Tid);
                spt.Assign = 1;
            }
            listchange.AddRange(sptlist);


        }
        /// <summary>
        /// 课堂抽查次数归零
        /// </summary>
        private void ResetClases()
        {
            foreach (ClassesModel m in listclasses)
            {
                m.CheckNumber = 0;
            }
        }
        /// <summary>
        /// 空闲时间已安排-->未安排
        /// </summary>
        private void ResetSpareTime()
        {
            foreach (SpareTimeModel m in listsparetime)
            {
                m.Assign = 0;
            }
        }
        /// <summary>
        /// 督导的总听课次数归零
        /// </summary>
        private void ResetCheckClass()
        {
            foreach (CheckClassModel m in listcheckcount)
            {
                m.DayNumber = 0;
                m.WeekNumber = 0;
                m.total = 0;
            }
        }
      
        /// <summary>
        /// 删除临时数据
        /// </summary>
        /// <returns>大于1-->成功;等于0失败</returns>
        private int ResetArrageData()
        {
            return DBHelper.ExecuteNonQuery(string.Format("delete from arrage where Stauts={0}",0),CommandType.Text,null);
        }
        public static void Test()
        {
            TeachersModel model = new TeachersModel();
            model.College = "信息工程学院";
            model.Email = "823894716@qq.com";
            model.Indentify = 1;
            model.Islimit = 1;
            model.Password = "123456";
            model.Phone = "13650421544";
            model.TeacherName = "刘莉(讲师（高校）)";
            model.Tid = "1994";
            model.Title = "教授";
            model.TeacherRoom = "物理教研室";
            DBHelper.Update<TeachersModel>(model);


        }
    }
}