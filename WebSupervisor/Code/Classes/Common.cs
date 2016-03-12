using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Data;
using WebDAL;

namespace WebSupervisor.Code.Classes
{
    public class jsondata
    {
        public int code;
        public string msg;
        public jsondata(int _code,string _msg)
        {
            code = _code;
            msg = _msg;
        }
    }
     public class Common
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfPath
        {
            get
            {
                return "~/App_Start/UserData.xml";
            }
        }
        public static int Year, Month, Day;//年月日
        #region xml操作

        /// <summary>
        /// 保存xml
        /// </summary>
        public  void xmlSave(string path)
        {
        
            XElement xe = new XElement("Config",
               new XElement("Year", Year.ToString()),
               new XElement("Month", Month.ToString()),
               new XElement("Day", Day.ToString())
              
               );
            xe.Save(path);
            xe.RemoveAll();
        }
        /// <summary>
        /// 读取xml
        /// </summary>
        public  void xmlRead(string path)
        {
            
                if (File.Exists(path))
                {
                    XElement xe = XElement.Load(path);

                    Year = Convert.ToInt32(xe.Element("Year").Value);
                    Month = Convert.ToInt32(xe.Element("Month").Value);
                    Day = Convert.ToInt32(xe.Element("Day").Value);

                   

                    xe.RemoveAll();

                   
                }
              
        }
        #endregion

       
        #region 输出Word文档
        public static void load_supervisor()
        {
            string Path = Environment.CurrentDirectory + "\\" + "supervisor.doc";
            if (!(File.Exists(Path)))
            {
                FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);

                try
                {
                    Byte[] b = Properties.Resources.supervisor;

                    fs.Write(b, 0, b.Length);
                    if (fs != null)
                        fs.Close();
                }
                catch
                {
                    if (fs != null)
                        fs.Close();

                }
            }
        }
        public static void load_cheif_supervisor()
        {
            string Path = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";
            if (!(File.Exists(Path)))
            {
                FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);

                try
                {
                    Byte[] b = Properties.Resources.chief_supervisor;

                    fs.Write(b, 0, b.Length);
                    if (fs != null)
                        fs.Close();
                    fs.Dispose();
                }
                catch
                {
                    if (fs != null)
                        fs.Close();
                    fs.Dispose();

                }
            }
        }

        public static void load_classes(string wordpath)
        {
            //string Path = Environment.CurrentDirectory + "\\" + "classes.docx";
            if (!(File.Exists(wordpath)))
            {
                FileStream fs = new FileStream(wordpath, FileMode.OpenOrCreate, FileAccess.Write);

                try
                {
                    Byte[] b = Properties.Resources.classes;

                    fs.Write(b, 0, b.Length);
                    if (fs != null)
                        fs.Close();
                }
                catch
                {
                    if (fs != null)
                        fs.Close();

                }
            }
        }
        #endregion
       
    }
}

