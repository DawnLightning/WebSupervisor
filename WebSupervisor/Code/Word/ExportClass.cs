using System;
using System.Collections.Generic;
using System.Data;
using WebSupervisor.Models;
using WebDAL;
namespace WebSupervisor.Code.Word
{   
    class ExportClass
    {   
        /// <summary>
        ///  使用说明
        ///  这里导出的数据默认为全部教师的所有上课情况，可以根据select语句筛选返回不同的DataTable
        ///  这样就可以实现：
        ///  1.导出一个教师的这个学期的上课课表
        ///  2.导出一门课程的所有上课情况
        ///  3.等等
        ///  ！所有节次在数据库中的存储都是数字，例如1-2节，那么就是12
        /// </summary>
        private List<ClassesModel> dtclass=new List<ClassesModel>();//进度表
        private List<ExportClassModel> Info = new List<ExportClassModel>();//对应教师的上课信息
        //private SqlHelper help = new SqlHelper();
        /// <summary>
        /// 从数据库中选择要导出的教学进度
        /// </summary>
       public bool  InitData(string condition)
        {    string selectcommand="";
            if (condition!="")
            {
                selectcommand = "select teachername,classtype,week,day,classnumber,classname from classes" + " where " + condition;
            }
            else
            {
                selectcommand = "select teachername,classtype,week,day,classnumber,classname from classes";
            }
            dtclass = DBHelper.ExecuteList<ClassesModel>(selectcommand, CommandType.Text, null);
            if (dtclass.Count==0)
            {
                return false;
            } 
            else
            {
                return true;
            }
        }
        /// <summary>
        /// //将数据库中的记录导入到对象数组中
        /// </summary>
        public void InitInfo()
        {   
            for (int i = 0; i < dtclass.Count;i++ )
            {
                ExportClassModel info = new ExportClassModel();
                info.Teachername = dtclass[i].TeacherName;
                info.Classtype = dtclass[i].ClassType;
                info.Week = dtclass[i].Week;
                info.Day =dtclass[i].Day;
                info.Classname = dtclass[i].ClassName;
                if ( dtclass[i].ClassNumber<100&& dtclass[i].ClassNumber>0)
                {
                    info.Start = dtclass[i].ClassNumber / 10;//例如89节，那/10就是8，%10就是9
                    info.End = dtclass[i].ClassNumber % 10;
                    if (info.End-info.Start>1)
                    {
                        info.IsOverTop = true;
                    } 
                    else
                    {
                        info.IsOverTop = false;
                    }
                } 
                else
                {
                    info.Start = dtclass[i].ClassNumber / 100;
                    info.End = dtclass[i].ClassNumber % 100;
                    if (info.End - info.Start > 1)
                    {
                        info.IsOverTop = true;
                    }
                    else
                    {
                        info.IsOverTop = false;
                    }
                }
                Info.Add(info);
            }
        }
        /// <summary>
        /// 输出word文档
        /// </summary>
        public bool MakeWordDoc(string selectcommand,string filename)
        {
            if (InitData(selectcommand)) //从数据库中选择要导出的教学进度
           {
               InitInfo();//将数据库中的记录导入到对象数组中
               WordTools tools = new WordTools();
               tools.fullclasses(Info, filename);//将对象数组写进word文档
                return true;
           }else{
                return false;
           }
         
           
        }
    }
}
