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
using WebSupervisor.Code.Classes;

namespace WebSupervisor
{
    class ExcelHelper
    {

        DataTable Excel_dt;
        List<ClassesModel> list = new List<ClassesModel>();
        List<TeachersModel> teacherlist = new List<TeachersModel>();
        List<CheckClassModel> checkclasslist = new List<CheckClassModel>();
        List<string> dcs = new List<string> { "序号", "课程", "授课内容", "授课方式", "专业", "教室", "教师", "周次", "听课时间", "听课人员安排", "分数", "申报" };//表格的列头信息
        List<string> ListSupervisor = new List<string>();//暂存分离出的督导员
        #region 读取教学进度表
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param Name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public int Import(string strFileName, string college)
        {
            try
            {
                Common com = new Common();
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
                string classname = Excel_dt.Rows[2][0].ToString();//课程名称
                string major = Excel_dt.Rows[2][1].ToString().Substring(3);//专业
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
                                //获取节次  
                                string strclassname = Excel_dt.Rows[i][name[2]].ToString();

                                int classnumindex = strclassname.IndexOf("-");
                                string tname = ClearTechnicalTitle(teachernamepick);
                                string tid = collegeid(college) + com.Get16binary(tname);
                                //if (i == 7)
                                //{
                                    SqlParameter[] sp = new SqlParameter[3];
                                    sp[0] = new SqlParameter("@tid", tid);
                                    sp[1] = new SqlParameter("@teachername", tname);
                                    sp[2] = new SqlParameter("@college", college);
                                    DBHelper.ExecuteNonQuery("INSERT INTO [dbo].[teachers] ([tid], [teachername],[college]) VALUES (@tid,@teachername,@college)", CommandType.Text, sp);
                                    //}
                                //    TeachersModel te = new TeachersModel();
                                //te.Tid = tid;
                                //te.TeacherName = tname;
                                //te.Phone = "0";
                                ClassesModel model = new ClassesModel();
                                model.Cid = tid + Excel_dt.Rows[i][name[0]].ToString() + j.ToString() + strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1);
                                model.Day = j;
                                model.ClassNumber = Convert.ToInt32(strclassname.Substring(0, classnumindex) + strclassname.Substring(classnumindex + 1));
                                model.ClassType = Excel_dt.Rows[i][name[6]].ToString();
                                model.Address = Excel_dt.Rows[i][name[3]].ToString();
                                model.TeacherName = tname;
                                model.Week = Convert.ToInt32(Excel_dt.Rows[i][name[0]]);
                                model.ClassName = classname.Substring(5);
                                model.ClassContent = Excel_dt.Rows[i][name[5]].ToString();
                                model.CheckNumber = 0;
                                model.Major = major;
                                list.Add(model);
                                //DBHelper.ExecuteNonQuery("INSERT INTO [dbo].[teachers] ([tid], [teachername], [phone], [email], [college], [indentify], [title], [islimit], [password], [teacherroom]) VALUES ("+''N'39', N'黄晓丽', N'13929462568', N' ', N'第二临床医学院', 0, N' ', 1, N'123', N' ') ", CommandType.Text, null);
                                //DBHelper.Insert<TeachersModel>(te);
                                //teacherlist.Add(te);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
                //DBHelper.BulkInsert<TeachersModel>(teacherlist);
                DBHelper.BulkInsert<ClassesModel>(list);

                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        private int Teacher(string strTeacher)
        {
            return strTeacher.Length - strTeacher.Replace(",", "").Length;
        }

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
        #endregion

        #region 读取教师信息表
        /// <summary>
        /// 教师信息导入
        /// 密码默认123
        /// 邮箱,教研室,职称为空
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="college">学院</param>
        public void ReadTeacherTable(string filename, string college)
        {
            Common com = new Common();
            Excel_dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
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

            for (int i = 4; i <= sheet.LastRowNum; i++)
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
            for (int i = 0; i < Excel_dt.Rows.Count; i++)
            {
                if (Excel_dt.Rows[i][0].ToString().Length != 0 && Excel_dt.Rows[i][1].ToString().Length == 11)
                {
                    string tname = Excel_dt.Rows[i][0].ToString();
                    TeachersModel m = new TeachersModel();
                    m.Tid = collegeid(college) +com.Get16binary(tname)  ;                    
                    m.TeacherName = Excel_dt.Rows[i][0].ToString();
                    m.Phone = Excel_dt.Rows[i][1].ToString();
                    if (Excel_dt.Rows[i][2].ToString() != null && Excel_dt.Rows[i][2].ToString() == "1")
                    {
                        m.Indentify = 1;
                        CheckClassModel c = new CheckClassModel();
                        c.Tid = collegeid(college) + com.Get16binary(tname);
                        c.DayNumber = 0;
                        c.WeekNumber = 0;
                        c.total = 0;
                        //checkclasslist.Add(c);
                        DBHelper.Insert<CheckClassModel>(c);
                    }
                    else
                    {
                        m.Indentify = 0;
                    }
                    m.Islimit = 1;
                    m.Password = "123";
                    m.TeacherRoom = " ";
                    m.Title = " ";
                    m.Email = " ";
                    m.College = college;
                    DBHelper.Insert<TeachersModel>(m);
                    //teacherlist.Add(m);
                }


            }
            DBHelper.BulkInsert<TeachersModel>(teacherlist);
            DBHelper.BulkInsert<CheckClassModel>(checkclasslist);
        }
        #endregion
        #region 导出安排表excel表格
        /// <summary>
        /// List<ExportExcelModel> dtSource 是表格需要的数据，只需要将内容放到对象就可以
        /// </summary>
        /// <param Name="dtSource">List<ExportExcelModel> dtSource 是表格需要的数据，只需要将内容放到对象就可以</param>
        /// <param Name="strHeaderText">Excel表头文本（例如：信息工程学院2014－2015学年第一学期教学检查听课安排）</param>
        /// <param Name="strFileName">保存位置</param>
        public void Export(List<ExportExcelModel> dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }



        /// <summary>
        /// DataTable导出到Excel的MemoryStream Export()
        /// </summary>
        /// <param Name="dtSource">DataTable数据源</param>
        /// <param Name="strHeaderText">Excel表头文本（例如：信息工程学院2014－2015学年第一学期教学检查听课安排）</param>
        private MemoryStream Export(List<ExportExcelModel> dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

          
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "破晓技术团队";
                workbook.DocumentSummaryInformation = dsi;
                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "管理员名册"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
            }
           

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            int rowIndex = 0;
            int j = 0;
            foreach (ExportExcelModel row in dtSource)
            {
                // 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    // 表头及样式
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; // ------------------
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 800;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dcs.Count - 1)); // ------------------
                    }


                    //
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; // ------------------
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 800;
                        font.FontName = "宋体";
                        headStyle.SetFont(font);
                        int index = 0;
                        foreach (string column in dcs)
                        {
                            headerRow.CreateCell(index).SetCellValue(column);
                            headerRow.GetCell(index).CellStyle = headStyle;
                            sheet.AutoSizeColumn(index);
                            //设置列宽
                            //sheet.SetColumnWidth(index, (arrColWidth[index] + 1) * 256);
                            //sheet.SetColumnWi
                            index++;
                        }
                    }


                    rowIndex = 2;
                }


                //填充内容

                IRow dataRow = sheet.CreateRow(rowIndex);

                int columnIndex = 0;
                //！！！！！！！！！！！！！！！！！！！！！！！如果没有职称可能会异常
                DistinctSupervisor(dtSource[j].supervisors, ListSupervisor);//去督导员的职称
                for (int i = 0; i < dcs.Count; i++)
                {
                    ICell newCell = dataRow.CreateCell(columnIndex);
                    columnIndex++;


                    if (i != dcs.Count - 1)
                    {

                        //注意这个在导出的时候加了“\t” 的目的就是避免导出的数据显示为科学计数法。可以放在每行的首尾。
                        switch (i)
                        {

                            case 0:
                                //序号
                                newCell.SetCellValue((j + 1).ToString() + "\t");

                                break;
                            case 1:
                                //课程
                                newCell.SetCellValue(dtSource[j].classname.ToString() + "\t");

                                break;
                            case 2:
                                //授课内容
                                newCell.SetCellValue(dtSource[j].classcontent.ToString() + "\t");

                                break;
                            case 3:
                                //授课方式
                                newCell.SetCellValue(dtSource[j].classtype.ToString() + "\t");

                                break;
                            case 4:
                                //专业
                                newCell.SetCellValue(dtSource[j].major.ToString() + "\t");

                                break;
                            case 5:
                                //教室
                                newCell.SetCellValue(dtSource[j].classroom.ToString() + "\t");

                                break;
                            case 6:
                                //教师
                                newCell.SetCellValue(dtSource[j].teachername.ToString() + "\t");

                                break;
                            case 7:
                                //周次
                                newCell.SetCellValue(dtSource[j].week.ToString() + "\t");

                                break;
                            case 8:
                                //听课时间

                                newCell.SetCellValue(dtSource[j].time+ "\t");

                                break;
                            case 9:
                                //听课人员安排
                                newCell.SetCellValue(FormatSupervisor(ListSupervisor) + "\t");

                                break;
                            case 10:
                                //分数
                                newCell.SetCellValue(" " + "\t");

                                break;
                        }

                    }
                    else
                    {
                        //申报
                        newCell.SetCellValue(" ");

                    }

                }
                j++;
                rowIndex++;
            }
            adjustcolum(sheet);//调整列宽
            AddBorder(sheet, workbook);//加边框
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                return ms;
            }
        }

        /// <summary>
        /// 加边框
        /// </summary>
        /// <param Name="rowindex">1开始</param>
        /// <param Name="cellIndex">1开始</param>
        private  void AddBorder(ISheet sheet, HSSFWorkbook workbook)

        {
            ICellStyle styel = workbook.CreateCellStyle();
            styel.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; // ------------------
            IFont font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 11;
            font1.Boldweight = 600;
            font1.FontName = "宋体";
            styel.SetFont(font1);
            for (int rowindex = 1; rowindex < sheet.LastRowNum + 1; rowindex++)
            {
                for (int cellIndex = 0; cellIndex < dcs.Count; cellIndex++)
                {
                    sheet.GetRow(rowindex).RowStyle = styel;
                    ICell cell = sheet.GetRow(rowindex).GetCell(cellIndex);

                    HSSFCellStyle Style = workbook.CreateCellStyle() as HSSFCellStyle;
                    Style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    Style.VerticalAlignment = VerticalAlignment.Center;
                    Style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    Style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    Style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    Style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    Style.DataFormat = 0;
                    Style.SetFont(font1);
                    cell.CellStyle = Style;
                }
            }

        }
        /// <summary>
        /// 自动调整列宽自适应
        /// </summary>
        /// <param name="sheet">工作表对象</param>
        private void adjustcolum(ISheet sheet)
        {
            for (int columnNum = 0; columnNum < dcs.Count; columnNum++)
            {
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;//获取当前列宽度  
                for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)//在这一列上循环行  
                {
                    IRow currentRow = sheet.GetRow(rowNum);
                    ICell currentCell = currentRow.GetCell(columnNum);

                    int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;//获取当前单元格的内容宽度  
                    if (columnWidth < length + 1)
                    {
                        columnWidth = length + 1;
                    }//若当前单元格内容宽度大于列宽，则调整列宽为当前单元格宽度，后面的+1是我人为的将宽度增加一个字符  
                }
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }
        }


        /// <summary>
        /// 将（小明，小马，小强）这些形式拆开
        /// </summary>
        /// <param name="supervisor"></param>
        /// <param name="ListSupervisor"></param>
        /// <returns></returns>
        private string DistinctSupervisor(string supervisor, List<string> ListSupervisor)
        {

            if (supervisor.IndexOf(",") != -1)
            {
                ListSupervisor.Add(supervisor.Substring(0, supervisor.IndexOf(",")));
                return DistinctSupervisor(supervisor.Substring(supervisor.IndexOf(",") + 1), ListSupervisor);
            }
            else
            {
                ListSupervisor.Add(supervisor);
                return supervisor;
            }

        }
        /// <summary>
        /// 连接督导成员（小明，小马，小强）这些形式
        /// </summary>
        /// <param name="List">督导组</param>
        /// <returns></returns>
        private string FormatSupervisor(List<string> List)
        {
            string supervisor = "";

            foreach (string s in List)
            {
                if (s.IndexOf("(") != -1)
                {
                    supervisor = supervisor + "," + s.Substring(0, s.IndexOf("("));
                }
                else
                {
                    supervisor = supervisor + "," + s;
                }
            }
            List.Clear();
            return supervisor.Substring(1);

        }

        /// <summary>
        ///  生成间位符"-"
        /// </summary>
        /// <param name="classnumber">给定的节次</param>
        /// <returns></returns>
        private string addseparator(int classnumber)
        {
            string newclassnumber = "";
            switch (classnumber)
            {
                case 12: newclassnumber = "1-2"; break;
                case 13: newclassnumber = "1-3"; break;
                case 23: newclassnumber = "2-3"; break;
                case 24: newclassnumber = "2-4"; break;
                case 34: newclassnumber = "3-4"; break;
                case 35: newclassnumber = "3-5"; break;
                case 45: newclassnumber = "4-5"; break;
                case 46: newclassnumber = "4-6"; break;
                case 67: newclassnumber = "6-7"; break;
                case 68: newclassnumber = "6-8"; break;
                case 78: newclassnumber = "7-8"; break;
                case 79: newclassnumber = "7-9"; break;
                case 89: newclassnumber = "8-9"; break;
                case 1011: newclassnumber = "10-11"; break;
                case 1112: newclassnumber = "11-12"; break;
                case 1012: newclassnumber = "10-12"; break;
            }
            return newclassnumber;
        }
        #endregion
        private string collegeid(string college)
        {
            switch (college)
            {
                case "研究生学院":
                    return "1";
                case "第一临床医学院":
                    return "2";
                case "第二临床医学院":
                    return "3";
                case "第三临床医学院":
                    return "4";
                case "公共卫生学院":
                    return "5";
                case "护理学院":
                    return "6";
                case "基础医学院":
                    return "7";
                case "外国语学院":
                    return "8";
                case "人文与管理学院":
                    return "9";
                case "信息工程学院":
                    return "10";
                case "药学院":
                    return "11";
                case "医学检验学院":
                    return "12";
                case "继续教育学院":
                    return "13";
                case "社会科学部":
                    return "14";
                case "体育教学部":
                    return "15";
                default:
                    return "未知";

            }

        }
    }


    public class ExportExcelModel{
        /// <summary>
        /// 课程名
        /// </summary>
         public string classname { set; get; }
        /// <summary>
        /// 授课内容
        /// </summary>
        public string classcontent { set; get; }
        /// <summary>
        /// 课程类型
        /// </summary>
        public string classtype { set; get; }
        /// <summary>
        /// 专业
        /// </summary>
        public string major { set; get; }
        /// <summary>
        /// 课室
        /// </summary>
        public string classroom { set; get; }
        /// <summary>
        /// 教师姓名
        /// </summary>
        public string teachername { set; get; }
        /// <summary>
        /// 上课周次
        /// </summary>
        public string week { set; get; }
        /// <summary>
        /// 上课时间
        /// </summary>
        public string time { set; get; }
        /// <summary>
        /// 督导成员
        /// </summary>
        public string supervisors { set; get; }
        
    }
}
