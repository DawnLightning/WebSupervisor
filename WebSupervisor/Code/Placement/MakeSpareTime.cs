using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSupervisor.Models;
using WebDAL;
using System.Data;
namespace WebSupervisor.Code.Placement
{   /// <summary>
    /// /此类用于自动生成空闲时间
    /// </summary>
    public class MakeSpareTime
    {
        static List<int> spareclass= new List<int> { 12, 13, 23, 24, 34, 35, 45, 67, 68, 78, 79, 89, 1011, 1112, 1012 };//枚举所有的连续节次
        /// <summary>
        ///为全部督导自动填补空闲时间
        /// </summary>
        public static void AutoSelectSpareTime()
        {
            List<TeachersModel> listsupervisor = DBHelper.ExecuteList<TeachersModel>("select * from teachers where indentify=1",CommandType.Text,null);//未审核过的督导
            List<ClassesModel> listclasses = DBHelper.ExecuteList<ClassesModel>("select * from classes",CommandType.Text,null);
            List<SpareTimeModel> listsparetime = new List<SpareTimeModel>();
            //分别把每一周每一天每一节的记录分开，存储在DrClassArray数组中
            for (int i = 0; i < listsupervisor.Count; i++)
            {
                if (SearchTecher(listclasses, listsupervisor[i].TeacherName))
                {
                    for (int week = 1; week < 20; week++)
                    {
                        for (int day = 1; day < 6; day++)
                        {
                            for (int index = 0; index < spareclass.Count; index++)
                            {
                                List<ClassesModel> dr = SelectClasses(listclasses, week, day, spareclass[index], listsupervisor[i].TeacherName.ToString());
                                if (dr.Count == 0)
                                {
                                    SpareTimeModel sptmodel = new SpareTimeModel();
                                    sptmodel.Tid = listsupervisor[i].Tid.ToString();
                                    sptmodel.Week = week;
                                    sptmodel.Day = day;
                                    sptmodel.ClassNumber = spareclass[index];
                                    sptmodel.Assign = 0;
                                    listsparetime.Add(sptmodel);
                                }
                                else
                                {
                                    CheckError(index, week, day, listsparetime);
                                }

                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            List<SpareTimeModel> newsupervisorlist = listsparetime.Distinct<SpareTimeModel>().ToList();
            DBHelper.BulkInsert<SpareTimeModel>(newsupervisorlist);
        }
        /// <summary>
        /// 指定某位督导并为其自动填补空闲时间
        /// </summary>
        /// <param name="teachername">督导姓名</param>
        public static void AutoSelectSpareTime(string teachername)
        {
            string selectcommand = string.Format("select * from teachers where indentify=1 and teachername='{0}'",teachername);
            List<TeachersModel> listsupervisor = DBHelper.ExecuteList<TeachersModel>(selectcommand, CommandType.Text, null);//未审核过的督导
            List<ClassesModel> listclasses = DBHelper.ExecuteList<ClassesModel>("select * from classes", CommandType.Text, null);
            List<SpareTimeModel> listsparetime = new List<SpareTimeModel>();
            //分别把每一周每一天每一节的记录分开，存储在DrClassArray数组中
            for (int i = 0; i < listsupervisor.Count; i++)
            {
                if (SearchTecher(listclasses, listsupervisor[i].TeacherName))
                {
                    for (int week = 1; week < 20; week++)
                    {
                        for (int day = 1; day < 6; day++)
                        {
                            for (int index = 0; index < spareclass.Count; index++)
                            {
                                List<ClassesModel> dr = SelectClasses(listclasses, week, day, spareclass[index], listsupervisor[i].TeacherName.ToString());
                                if (dr.Count == 0)
                                {
                                    SpareTimeModel sptmodel = new SpareTimeModel();
                                    sptmodel.Tid = listsupervisor[i].Tid.ToString();
                                    sptmodel.Week = week;
                                    sptmodel.Day = day;
                                    sptmodel.ClassNumber = spareclass[index];
                                    sptmodel.Assign = 0;
                                    listsparetime.Add(sptmodel);
                                }
                                else
                                {
                                    CheckError(index, week, day, listsparetime);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int week = 1; week < 20; week++)
                    {
                        for (int day = 1; day < 6; day++)
                        {
                            for (int index = 0; index < spareclass.Count; index++)
                            {
                                //List<ClassesModel> dr = SelectClasses(listclasses, week, day, spareclass[index], listsupervisor[i].TeacherName.ToString());
                                //if (dr.Count == 0)
                                //{
                                    SpareTimeModel sptmodel = new SpareTimeModel();
                                    sptmodel.Tid = listsupervisor[i].Tid.ToString();
                                    sptmodel.Week = week;
                                    sptmodel.Day = day;
                                    sptmodel.ClassNumber = spareclass[index];
                                    sptmodel.Assign = 0;
                                    listsparetime.Add(sptmodel);
                                //}
                                //else
                                //{
                                //    CheckError(index, week, day, listsparetime);
                                //}

                            }
                        }
                    }
                    continue;
                }
            }
            List<SpareTimeModel> newsupervisorlist = listsparetime.Distinct<SpareTimeModel>().ToList();
            DBHelper.BulkInsert<SpareTimeModel>(newsupervisorlist);
        }
        private static bool SearchTecher(List<ClassesModel> models,string teachername)
        {
            foreach (ClassesModel model in models)
            {
                if (model.TeacherName.Equals(teachername))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 删除错误的数据,配合checkerror使用
        /// </summary>
        /// <param name="num">枚举错误节次</param>
        /// <param name="thisweek">周次</param>
        /// <param name="thisday">天</param>
        /// <param name="list">目标list</param>
        private static void DeleteError(int[] num, int thisweek, int thisday, List<SpareTimeModel> list)
        {
            List<SpareTimeModel> newlist = new List<SpareTimeModel>();
            foreach (SpareTimeModel i in list)
            {
                newlist.Add(i);
            }
            for (int j = 0; j < num.Length; j++)
            {
                for (int i = 0; i < newlist.Count; i++)
                {
                    if (newlist[i].ClassNumber.Equals(num[j]) && newlist[i].Week.Equals(thisweek) && newlist[i].Day.Equals(thisday))
                    {
                        list.Remove(newlist[i]);
                    }
                }
            }
            newlist.Clear();
        }
        /// <summary>
        /// 从list选择出指定数据
        /// </summary>
        /// <param name="list">目标list</param>
        /// <param name="week">周次</param>
        /// <param name="day">天</param>
        /// <param name="classnumber">节次</param>
        /// <param name="name">督导姓名</param>
        /// <returns></returns>
        private static List<ClassesModel> SelectClasses(List<ClassesModel> list,int week,int day,int classnumber,string name)
        {
            List<ClassesModel> listclass = new List<ClassesModel>();
            foreach (ClassesModel model in list)
            {
                if (model.Week==week&&model.Day==day&&model.ClassNumber==classnumber&&model.TeacherName==name)
                {
                    listclass.Add(model);
                }
            }
            return listclass;
        }
        /// <summary>
        /// 生成测试数据
        /// </summary>
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
            model.Title = "讲师";
            model.TeacherRoom = "数学教研室";
            DBHelper.Insert<TeachersModel>(model);


        }
        /// <summary>
        /// 检测错误数据
        /// </summary>
        /// <param name="index">节次数组索引</param>
        /// <param name="week">周次</param>
        /// <param name="day">天</param>
        /// <param name="listsparetime">存储空闲信息的集合</param>
        private static void CheckError(int index,int week,int day,List<SpareTimeModel> listsparetime)
        {
            int[] array;
            switch (spareclass[index])
            {
                case 12:
                    index += 3;
                    break;
                case 13:
                    array = new int[] { 12 };
                    DeleteError(array, week, day, listsparetime);
                    index += 4;
                    break;
                case 23:
                    array = new int[] { 12, 13 };
                    DeleteError(array, week, day, listsparetime);
                    index += 3;
                    break;
                case 24:
                    array = new int[] { 12, 13, 23 };
                    DeleteError(array, week, day, listsparetime);
                    index += 3;
                    break;
                case 34:
                    array = new int[] { 13, 23, 24 };
                    DeleteError(array, week, day, listsparetime);
                    index += 2;
                    break;
                case 35:
                    array = new int[] { 13, 24, 23, 34 };
                    DeleteError(array, week, day, listsparetime);
                    index += 1;
                    break;
                case 45:
                    array = new int[] { 24, 34, 35 };
                    DeleteError(array, week, day, listsparetime);
                    break;
                case 67:
                    index += 3;
                    break;
                case 68:
                    array = new int[] { 67 };
                    DeleteError(array, week, day, listsparetime);
                    index += 3;
                    break;
                case 78:
                    array = new int[] { 67, 68 };
                    DeleteError(array, week, day, listsparetime);
                    index += 2;
                    break;
                case 79:
                    array = new int[] { 67, 78, 68 };
                    DeleteError(array, week, day, listsparetime);
                    index += 1;
                    break;
                case 89:
                    array = new int[] { 68, 79, 78 };
                    DeleteError(array, week, day, listsparetime);
                    break;
                case 1011:
                    index += 2;
                    break;
                case 1112:
                    array = new int[] { 1011, 1012 };
                    DeleteError(array, week, day, listsparetime);
                    index += 1;
                    break;
                case 1012:
                    array = new int[] { 1011, 1112 };
                    DeleteError(array, week, day, listsparetime);
                    break;

            }
        }
    }
}