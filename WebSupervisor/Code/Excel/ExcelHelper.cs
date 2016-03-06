using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using WebDAL;
using WebSupervisor.Models;

namespace WebSupervisor
{
    class ExcelHelper
    {

        DataTable Excel_dt;
        List<ClassesModel> list = new List<ClassesModel>();
        Random r = new Random();

        #region 读取excel ,默认第一行为标头Import()
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param Name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public int Import(string strFileName)
        {
            //try
            //{
            //    //List<ScheduleModle> lstschedule = new List<ScheduleModle>();
            //lstschedule = DBHelper.ExecuteList<ScheduleModle>(strSelect_Class_Data, CommandType.Text, null);
            //foreach (ScheduleModle schedule in lstschedule)
            //{

            //}
            Excel_dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {

                Excel_dt.Columns.Add(j.ToString());
            }
            DataRow title = Excel_dt.NewRow();//标题
            title[0] = sheet.GetRow(0).ToString();
            Excel_dt.Rows.Add(title);
            DataRow term = Excel_dt.NewRow();//学期
            term[0] = sheet.GetRow(1).ToString();
            Excel_dt.Rows.Add(term);
            DataRow classinfo = Excel_dt.NewRow();//课程专业
            classinfo[0] = sheet.GetRow(2).GetCell(0).ToString();
            classinfo[1] = sheet.GetRow(2).GetCell(4).ToString();
            Excel_dt.Rows.Add(classinfo);

            DataRow student = Excel_dt.NewRow();//年级和班级
            student[0] = sheet.GetRow(3).GetCell(0).ToString();
            student[1] = sheet.GetRow(3).GetCell(4).ToString();
            Excel_dt.Rows.Add(student);

            DataRow book = Excel_dt.NewRow();//教材
            book[0] = sheet.GetRow(4).GetCell(0).ToString();
            Excel_dt.Rows.Add(book);

            headerRow = sheet.GetRow(5);

            Random ran = new Random();
            int RandKey = ran.Next(0, 1000);
            for (int i = 5; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null)
                {
                    DataRow dataRow = Excel_dt.NewRow();

                    for (int j = 0; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j).ToString();
                    }
                    Excel_dt.Rows.Add(dataRow);
                }
                else
                {
                    break;
                }

            }

            int[] name = new int[7];//确定Excel的所需字段值所在的列---
                                    //daClass = helper.adapter(strSelect_Class_Data);
                                    //dtClass = new DataTable();
                                    //daClass.Fill(dtClass);
                                    //daClass.FillSchema(dtClass, SchemaType.Source);
            string classname = Excel_dt.Rows[2][0].ToString();//课程名称
            string major = Excel_dt.Rows[2][1].ToString().Substring(3);//专业
            //string banji = Excel_dt.Rows[3][1].ToString();
            for (int q = 0; q < Excel_dt.Columns.Count; q++)
            {
                switch (Excel_dt.Rows[5][q].ToString())
                {
                    case "周次": name[0] = q; break;
                    case "星期": name[1] = q; break;
                    case "节次": name[2] = q; break;
                    case "上课地点": name[3] = q; break;
                    case "授课教师": name[4] = q; break;
                    case "授课内容": name[5] = q; break;
                    case "授课方式": name[6] = q; break;
                }
            }

            //dt = dtClass.Copy();   //  获取Class_Data的架构
            //dt.Clear();
            for (int i = 6; i < Excel_dt.Rows.Count; i++)
            {
                /*dr = dt.rows[i];*///获取excel的当前操作行的数据
                if (!(Excel_dt.Rows[i][name[0]].ToString() == "周次" || Excel_dt.Rows[i][name[0]].ToString() == ""))
                {
                    string teachername = Excel_dt.Rows[i][name[4]].ToString();//获取授课老师列的数据
                    int k = Teacher(teachername) + 1;//判断有多少位老师上同一节课
                    for (int m = 1; m <= k; m++)//有几位老师，就循环几次
                    {
                        string teachernamepick;//定义截取的老师名字
                        //以逗号为分界点，把多位老师的名字分成各自的名字
                        if ((k == 1) || (m == k)) teachernamepick = teachername;
                        else
                        {
                            int index2 = teachername.IndexOf(",");
                            teachernamepick = teachername.Substring(0, index2);
                            teachername = teachername.Remove(0, index2 + 1);
                        }


                        int j;
                        //判断星期几，返回对应的数字
                        switch (Excel_dt.Rows[i][name[1]].ToString().Substring(0, 1))
                        {
                            case "一": j = 1; break;
                            case "二": j = 2; break;
                            case "三": j = 3; break;
                            case "四": j = 4; break;
                            case "五": j = 5; break;
                            default: j = 0; break;
                        }
                        if (teachernamepick != null && teachernamepick != "")
                        {

                            //ScheduleModle schedule = DBHelper.ExexuteEntity<ScheduleModle>(strSelect_Class_Data, CommandType.Text, null);
                            //获取节次  
                            string strclassname = Excel_dt.Rows[i][name[2]].ToString();

                            int classnumindex = strclassname.IndexOf("-");
                            //int tnum = teachernamepick.IndexOf("(");//获取教师姓名列中的第一个"("的位置
                            string tname = ClearTechnicalTitle(teachernamepick);
                            //if (tnum >= 0)
                            //    tname = teachernamepick.Substring(0, tnum);//截取老师名字
                            //else
                            //    tname = teachernamepick;
                            //int classnumindex = strclassname.IndexOf("-");
                            //schedule.Day = j;
                            //schedule.TeacherName = teachernamepick;
                            //schedule.Cid= teachernamepick + Excel_dt.Rows[i][name[0]].ToString() + j.ToString() + strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1) + classname.Substring(5) + Excel_dt.Rows[i][name[3]] + banji;
                            //schedule.Week = (int)Excel_dt.Rows[i][name[0]];
                            //schedule.ClassNumber= Convert.ToInt32(strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1));
                            //schedule.Address = (string)Excel_dt.Rows[i][name[3]];

                            //DataRow drClass_information = dt.NewRow();
                            ///--------------------------------------------------------------------------------------------                          
                            //string tid = DBHelper.ExexuteEntity<string>("select tid from teachers where teachername=" + tname, CommandType.Text, null);
                            string tid = RandKey.ToString();
                            ClassesModel model = new ClassesModel();
                            model.Cid = tid + Excel_dt.Rows[i][name[0]].ToString() + j.ToString() + strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1);
                            model.Day = j;
                            model.ClassNumber = Convert.ToInt32(strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1));
                            model.ClassType = Excel_dt.Rows[i][name[6]].ToString();
                            model.Address = Excel_dt.Rows[i][name[3]].ToString();
                            model.TeacherName = teachernamepick;
                            model.Week = Convert.ToInt32(Excel_dt.Rows[i][name[0]]);
                            model.ClassName = classname.Substring(5);
                            model.ClassContent = Excel_dt.Rows[i][name[5]].ToString();
                            model.CheckNumber = 0;
                            model.Major = major;
                            //DBHelper.Insert<ClassesModel>(model);
                            list.Add(model);
                            // DBHelper.Insert<ClassesModel>(model, "insert into classes values(@cid,@teachername,@classname,@classcontent,@classtype,@address,@week,@day,@classnumber,@checknumber)");
                            ///---------------------------------------------------------------------------------------------
                            //sqlparament[7] = new SqlParameter("@day", j);
                            //sqlparament[1] = new SqlParameter("@teachername", teachernamepick);
                            //sqlparament[2] = new SqlParameter("@classname", classname.Substring(5));
                            //sqlparament[0] = new SqlParameter("@cid", "00" + Excel_dt.Rows[i][name[0]].ToString() + j.ToString() + strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1));
                            //sqlparament[3] = new SqlParameter("@classcontent", Excel_dt.Rows[i][name[5]]);
                            //sqlparament[6] = new SqlParameter("@week", Excel_dt.Rows[i][name[0]]);
                            //sqlparament[5] = new SqlParameter("@address", Excel_dt.Rows[i][name[3]]);
                            //sqlparament[4] = new SqlParameter("@classtype", Excel_dt.Rows[i][name[6]]);
                            //sqlparament[8] = new SqlParameter("@classnumber", Convert.ToInt32(strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1)));
                            //sqlparament[9] = new SqlParameter("@checknumber", 1);
                            //DBHelper.ExecuteNonQuery("insert into classes values(@cid,@teachername,@classname,@classcontent,@classtype,@address,@week,@day,@classnumber,@checknumber)", CommandType.Text, sqlparament);
                            //drClass_information["day"] = j;


                            //drClass_information["Teachername"] = teachernamepick;
                            //drClass_information["CID"] = teachernamepick + Excel_dt.Rows[i][name[0]].ToString() + j.ToString() + strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1) + classname.Substring(5) + Excel_dt.Rows[i][name[3]] + banji;
                            //drClass_information["Teacher_ID"] = "0000000000";
                            //drClass_information["Class_Week"] = Excel_dt.Rows[i][name[0]];
                            //drClass_information["Class_Number"] = Convert.ToInt32(strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1));
                            //drClass_information["Class_Address"] = Excel_dt.Rows[i][name[3]];
                            //drClass_information["Class_Name"] = classname.Substring(5);
                            //drClass_information["Class_Content"] = Excel_dt.Rows[i][name[5]];
                            //drClass_information["Class_Type"] = Excel_dt.Rows[i][name[6]];
                            //drClass_information["Spcialty"] = spcialty;

                            //dt.Rows.Add(drClass_information);
                        }
                        else
                        {
                            return 0;
                        }

                    }
                }



            }
            //dtClass.Merge(dt, true);

            //daClass.Update(dtClass);
            DBHelper.BulkInsert<ClassesModel>(list);

            return 1;
            //}
            //catch (Exception)
            //{
            //    return 0;
            //}

        }
        private int Teacher(string strTeacher)
        {
            return strTeacher.Length - strTeacher.Replace(",", "").Length;
        }
        #endregion
        /// <summary>
        /// 去掉职称
        /// </summary>
        /// <param name="s">教师姓名</param>
        /// <returns></returns>
        private string ClearTechnicalTitle(string s)
        {
            if (s.IndexOf("(") != -1)
            {
                return s.Substring(0, s.IndexOf("("));
            }
            else
            {
                return s;
            }
        }
    }
}
