using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSupervisor.Code.Classes;
using WebSupervisor.Models;

namespace WebSupervisor.Code.Word
{
    class WordTools
    {

        private void closefile()//删除word文档线程
        {
            try
            {
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("WINWORD"))
                {
                    p.Kill();
                }
            }
            catch (Exception)
            {
            }
        }
        //首席
        //public string Addchiefsupervisordata(WordTableInfo Info)
        //{
        //    Common.Common.load_cheif_supervisor();
        //    object missingValue = System.Reflection.Missing.Value;
        //    object myTrue = false;                  //不显示Word窗口
        //    object fileName = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";//WORD文档所在路径
        //    string newfile = Common.Common.strAddfilesPath + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";//存储路径名称
        //    // object fileName1 = Environment.CurrentDirectory + "\\" + "supervisor.doc";
        //    Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application();
        //    Microsoft.Office.Interop.Word._Document oDoc;
        //    oDoc = oWord.Documents.Open(ref fileName, ref missingValue,
        //       ref myTrue, ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue);
        //    Microsoft.Office.Interop.Word.Table newtable = oDoc.Tables[1];//获取word文档中的表格
        //    newtable.Cell(1, 2).Range.Text = Info.Teacher;
        //    newtable.Cell(1, 6).Range.Text = Info.Time.Substring(0, Info.Time.IndexOf(" "));
        //    newtable.Cell(2, 6).Range.Text = Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1);
        //    newtable.Cell(4, 2).Range.Text = Info.Class;
        //    newtable.Cell(5, 2).Range.Text = Info.Subject;
        //    object bSaveChange = true;
        //    oDoc.Close(ref bSaveChange, ref missingValue, ref missingValue);
        //    oDoc = null;
        //    oWord = null;

        //    closefile();
        //    if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
        //    {
        //        Directory.CreateDirectory(Common.Common.strAddfilesPath);
        //    }

        //    System.IO.File.Copy(fileName.ToString(), newfile, true);

        //    File.Delete(fileName.ToString());
        //    //sent_email(Supervisor, Time, Subject, newfile);
        //    //movetofile(newfile);
        //    return newfile;
        //}
        ////一般
        //public string fullcheifsupervisor(WordTableInfo Info)
        //{
        //    Common.Common.load_cheif_supervisor();
        //    string fileName = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";//WORD文档所在路径
        //    string newfile = Common.Common.strAddfilesPath + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";//存储路径名称
        //    DocX doc = DocX.Load(fileName);
        //    Table table = doc.Tables[0];

        //    table.Rows[0].Cells[1].Paragraphs[0].ReplaceText("teacher", Info.Teacher);
        //    table.Rows[0].Cells[5].Paragraphs[0].ReplaceText("time", Info.Time.Substring(0, Info.Time.IndexOf(" ")));
        //    table.Rows[1].Cells[5].Paragraphs[0].ReplaceText("address", Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1));
        //    table.Rows[3].Cells[1].Paragraphs[0].ReplaceText("class", Info.Class);
        //    table.Rows[4].Cells[1].Paragraphs[0].ReplaceText("context", Info.Subject);
        //    if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
        //    {
        //        Directory.CreateDirectory(Common.Common.strAddfilesPath);
        //    }
        //    doc.SaveAs(newfile);

        //    doc.Dispose();
        //    return newfile;
        //}
        //public string fullsupervisor(WordTableInfo Info)
        //{
        //    Common.Common.load_supervisor();
        //    string fileName1 = Environment.CurrentDirectory + "\\" + "supervisor.doc";
        //    string newfile = Common.Common.strAddfilesPath + "\\" + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";
        //    DocX doc = DocX.Load(fileName1);
        //    doc.ReplaceText("title", "广东医学院教师课堂教学质量评价表" + "(" + Info.Teachingtype + ")");
        //    Table table = doc.Tables[0];
        //    table.Rows[0].Cells[1].Paragraphs[0].ReplaceText("Teacher", Info.Teacher);
        //    table.Rows[0].Cells[3].Paragraphs[0].ReplaceText("Perfession", Info.Perfession);
        //    table.Rows[0].Cells[5].Paragraphs[0].ReplaceText("Time", Info.Time.Substring(0, Info.Time.IndexOf(" ")));
        //    table.Rows[1].Cells[1].Paragraphs[0].ReplaceText("Class", Info.Class);
        //    table.Rows[1].Cells[3].Paragraphs[0].ReplaceText("address", Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1));
        //    table.Rows[2].Cells[1].Paragraphs[0].ReplaceText("Context", Info.Subject);
        //    if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
        //    {
        //        Directory.CreateDirectory(Common.Common.strAddfilesPath);
        //    }
        //    doc.SaveAs(newfile);
        //    doc.Dispose();
        //    return newfile;
        //}
        /// <summary>
        /// 将对象数组写入word文档
        /// </summary>
        /// <param name="info">对象数组</param>
        public string fullclasses(List<ExportClassModel> info,string classespath,string wordpath)
        {
            Common.load_classes(wordpath);//加载输出word文档，可以在resource文件中查看
            string fileName1 = wordpath;//输出目录
            string newfile = classespath;//保存目录
            DocX doc = DocX.Load(fileName1);//用第三方类库加载word文档
            //去除表格中的数字
            for (int i = 0; i < doc.Tables.Count; i++)
            {
                Table tb = doc.Tables[i];
                for (int column = 2; column < 7; column++)
                {
                    for (int row = 1; row < 12; row++)
                    {
                        string text = tb.Rows[row].Cells[column].Paragraphs[0].Text;
                        if (IsNumber(text))
                        {
                            tb.Rows[row].Cells[column].Paragraphs[0].ReplaceText(text, "");
                        }


                    }
                }
            }
            for (int i = 0; i < info.Count; i++)
            {
                Table tb = doc.Tables[info[i].Week - 1];//word文档总共有表格20张，例如：现在是第一周，那么就是第一张表，索引从0开始
                //通过表格原有的数字，确定替换位置，例如：现在是第1周，星期1，第1-2节，那么对应的表格位置就是第2行，第2列，仔细看resource中的表格
                if (info[i].Start < 10)//上下午
                {

                    tb.Rows[info[i].Start].Cells[1 + info[i].Day].Paragraphs[0].AppendLine(info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");

                    tb.Rows[info[i].End].Cells[1 + info[i].Day].Paragraphs[0].AppendLine(info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                    if (info[i].IsOverTop)
                    {
                        for (int k = info[i].Start + 1; k < info[i].End; k++)
                        {

                            tb.Rows[k].Cells[1 + info[i].Day].Paragraphs[0].AppendLine(info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                        }

                    }
                }
                else//晚上
                {

                    tb.Rows[info[i].Start].Cells[1 + info[i].Day].Paragraphs[0].AppendLine(info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                    if (info[i].IsOverTop)
                    {
                        for (int k = info[i].Start + 1; k < info[i].End; k++)
                        {

                            tb.Rows[k].Cells[1 + info[i].Day].Paragraphs[0].AppendLine(info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                        }
                    }
                }
            }

            //if (!File.Exists(classespath))
            //{
            //    Directory.CreateDirectory(classespath);
            //}
            try
            {
                doc.SaveAs(newfile);//保存
                doc.Dispose();//释放对象
                //MessageBox.Show("导出成功");
                return "导出成功";
            }
            //catch (IOException)
            //{
            //    //MessageBox.Show("当前文件正在打开，请关闭后重试");
            //    return "当前文件正在打开，请关闭后重试";
            //}
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                return e.ToString();
            }

        }
        private bool IsNumber(string text)
        {
            try
            {
                Convert.ToInt32(text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public string Addsupervisordata(WordTableInfo Info)
        //{
        //    Common.Common.load_supervisor();
        //    object missingValue = System.Reflection.Missing.Value;
        //    object myTrue = false;                  //不显示Word窗口
        //    //object fileName = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";
        //    object fileName1 = Environment.CurrentDirectory + "\\" + "supervisor.doc";
        //    string newfile = Common.Common.strAddfilesPath + "\\" + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";
        //    Microsoft.Office.Interop.Word._Application oWord1 = new Microsoft.Office.Interop.Word.Application();
        //    Microsoft.Office.Interop.Word._Document oDoc1;
        //    oDoc1 = oWord1.Documents.Open(ref fileName1, ref missingValue,
        //       ref myTrue, ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue, ref missingValue, ref missingValue,
        //       ref missingValue);
        //    Microsoft.Office.Interop.Word.Table newtable1 = oDoc1.Tables[1];
        //    oWord1.Selection.TypeText("广东医学院教师课堂教学质量评价表" + "(" + Info.Teachingtype + ")");
        //    newtable1.Cell(1, 2).Range.Text = Info.Teacher;
        //    newtable1.Cell(1, 4).Range.Text = Info.Perfession;
        //    newtable1.Cell(1, 6).Range.Text = Info.Time.Substring(0, Info.Time.IndexOf(" "));
        //    newtable1.Cell(2, 2).Range.Text = Info.Class;
        //    newtable1.Cell(2, 4).Range.Text = Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1);
        //    newtable1.Cell(3, 2).Range.Text = Info.Subject;
        //    object bSaveChange = true;
        //    oDoc1.Close(ref bSaveChange, ref missingValue, ref missingValue);
        //    oDoc1 = null;
        //    oWord1 = null;



        //    closefile();
        //    if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
        //    {
        //        Directory.CreateDirectory(Common.Common.strAddfilesPath);
        //    }
        //    System.IO.File.Copy(fileName1.ToString(), newfile, true);

        //    File.Delete(fileName1.ToString());
        //    return newfile;

        //}
    }
}
