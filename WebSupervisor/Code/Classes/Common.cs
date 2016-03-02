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
     public class Common
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        //const string strDatabaseName = "DataBase.mdb";
        //public static string TempPath = System.IO.Path.GetTempPath() + "SAS";
        //static string MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ///// <summary>
        ///// 数据库路径
        ///// </summary>
        //public static string strDatabasePath
        //{
        //    get
        //    {
        //        return MyDocumentsPath + "\\SAS\\" + strDatabaseName;
        //    }
        //}

        //public static string strEmailResultPath
        //{
        //    get
        //    {
        //        return MyDocumentsPath + "\\SAS\\EmailResult\\";
        //    }
        //}
        //public static string strAddfilesPath
        //{
        //    get
        //    {
        //        return MyDocumentsPath + "\\SAS\\Addfiles\\";
        //    }
        //}
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
            //string path = Path.GetDirectoryName(Common.ConfPath);
            ////if (!Directory.Exists(path))//判断是否存在
            //{
            //    Directory.CreateDirectory(path);//创建新路径
            //}
            XElement xe = new XElement("Config",
               new XElement("Year", Year.ToString()),
               new XElement("Month", Month.ToString()),
               new XElement("Day", Day.ToString())
               //new XElement("MailAddress", MailAddress),
               //new XElement("MailPassword", MailPassword)
               );
            xe.Save(path);
            xe.RemoveAll();
        }
        /// <summary>
        /// 读取xml
        /// </summary>
        public  void xmlRead(string path)
        {
            //try
            //{
                if (File.Exists(path))
                {
                    XElement xe = XElement.Load(path);

                    Year = Convert.ToInt32(xe.Element("Year").Value);
                    Month = Convert.ToInt32(xe.Element("Month").Value);
                    Day = Convert.ToInt32(xe.Element("Day").Value);

                    //MailAddress = xe.Element("MailAddress").Value;
                    //MailPassword = xe.Element("MailPassword").Value;

                    xe.RemoveAll();

                    //if (string.IsNullOrWhiteSpace(Common.MailAddress) || string.IsNullOrWhiteSpace(Common.MailPassword))
                    //{
                    //    SAS.Forms.frmSetting fs = new Forms.frmSetting();
                    //    fs.ShowDialog();
                    //}
                }
                //else
                //{
                //    //SAS.Forms.frmSetting fs = new Forms.frmSetting();
                //    //fs.ShowDialog();
                //}


            //}
            //catch  // (Exception ex)
            //{
            //    //SAS.Forms.frmSetting fs = new Forms.frmSetting();
            //    //fs.ShowDialog();
            //}
        }
        #endregion

        //#region 数据库操作

        
        //#endregion
        //    public static int Year, Month, Day;//年月日
        //    public static int Week//周数
        //    {
        //        get
        //        {    
        //            return CalendarTools.weekdays(CalendarTools.CaculateWeekDay(Year, Month, Day));
        //        }
        //    }
        //    public static string MailAddress, MailPassword;//发送者邮箱账号密码


        //    #region xml操作

        //    /// <summary>
        //    /// 保存xml
        //    /// </summary>
        //    public static void xmlSave()
        //    {
        //        string path = Path.GetDirectoryName(Common.ConfPath);
        //        if (!Directory.Exists(path))//判断是否存在
        //        {
        //            Directory.CreateDirectory(path);//创建新路径
        //        }
        //        XElement xe = new XElement("Config",
        //           new XElement("Year", Year.ToString()),
        //           new XElement("Month", Month.ToString()),
        //           new XElement("Day", Day.ToString()),
        //           new XElement("MailAddress", MailAddress),
        //           new XElement("MailPassword", MailPassword)
        //           );
        //        xe.Save(Common.ConfPath);
        //        xe.RemoveAll();
        //    }
        //    /// <summary>
        //    /// 读取xml
        //    /// </summary>
        //    public static void xmlRead()
        //    {
        //        try
        //        {
        //            if (File.Exists(Common.ConfPath))
        //            {
        //                XElement xe = XElement.Load(Common.ConfPath);

        //                Year = Convert.ToInt32(xe.Element("Year").Value);
        //                Month = Convert.ToInt32(xe.Element("Month").Value);
        //                Day = Convert.ToInt32(xe.Element("Day").Value);

        //                MailAddress = xe.Element("MailAddress").Value;
        //                MailPassword = xe.Element("MailPassword").Value;

        //                xe.RemoveAll();

        //                if (string.IsNullOrWhiteSpace(Common.MailAddress) || string.IsNullOrWhiteSpace(Common.MailPassword))
        //                {
        //                    SAS.Forms.frmSetting fs = new Forms.frmSetting();
        //                    fs.ShowDialog();
        //                }
        //            }
        //            else
        //            {
        //                SAS.Forms.frmSetting fs = new Forms.frmSetting();
        //                fs.ShowDialog();
        //            }


        //        }
        //        catch  // (Exception ex)
        //        {
        //            SAS.Forms.frmSetting fs = new Forms.frmSetting();
        //            fs.ShowDialog();
        //        }
        //    }
        //    #endregion
        //    #region 输出数据库   
        //     public static bool load()
        //    {
        //        //---------------------------------输出数据库---------------------------

        //        if (!(File.Exists(strDatabasePath)))
        //        {
        //            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SAS\\";
        //            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //            FileStream fs = new FileStream(strDatabasePath, FileMode.OpenOrCreate, FileAccess.Write);

        //            try
        //            {
        //                Byte[] b = SAS.Properties.Resources.DataBase;

        //                fs.Write(b, 0, b.Length);
        //                if (fs != null)
        //                    fs.Close();
        //            }
        //            catch
        //            {
        //                if (fs != null)
        //                    fs.Close();
        //                return false;
        //            }
        //        }
        //        // -----------------------------第一次加载代码，------------------------------
        //        //if (conn.State != ConnectionState.Open)
        //        //    conn.Open();

        //        ////-----------------------------操作完成后记得关闭连接------------------------------
        //        //conn.Close();
        //        return true;
        //    }
        //    #endregion
        //    #region 输出Word文档
        //     public  static void load_supervisor()
        //     {
        //         string Path = Environment.CurrentDirectory + "\\" + "supervisor.doc";
        //         if (!(File.Exists(Path)))
        //         {
        //             FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);

        //             try
        //             {
        //                 Byte[] b = SAS.Properties.Resources.supervisor;

        //                 fs.Write(b, 0, b.Length);
        //                 if (fs != null)
        //                     fs.Close();
        //             }
        //             catch
        //             {
        //                 if (fs != null)
        //                     fs.Close();

        //             }
        //         }
        //     }
        //     public static void load_cheif_supervisor()
        //     {
        //         string Path = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";
        //         if (!(File.Exists(Path)))
        //         {
        //             FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);

        //             try
        //             {
        //                 Byte[] b = SAS.Properties.Resources.chief_supervisor;

        //                 fs.Write(b, 0, b.Length);
        //                 if (fs != null)
        //                     fs.Close();
        //                 fs.Dispose();
        //             }
        //             catch
        //             {
        //                 if (fs != null)
        //                     fs.Close();
        //                 fs.Dispose();

        //             }
        //         }
        //     }

        //     public static void load_classes()
        //     {
        //         string Path = Environment.CurrentDirectory + "\\" + "classes.docx";
        //         if (!(File.Exists(Path)))
        //         {
        //             FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);

        //             try
        //             {
        //                 Byte[] b = SAS.Properties.Resources.classes;

        //                 fs.Write(b, 0, b.Length);
        //                 if (fs != null)
        //                     fs.Close();
        //             }
        //             catch
        //             {
        //                 if (fs != null)
        //                     fs.Close();

        //             }
        //         }
        //     }
        //    #endregion
        //}
    }
}

