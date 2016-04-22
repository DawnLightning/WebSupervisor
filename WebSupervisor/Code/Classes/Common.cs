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
    public class mkjson
    {
        public int code;
        public object data;
        public string msg;
        public mkjson(string _msg, int _code = 0, object _data = null)
        {
            msg = _msg;
            code = _code;
            data = _data;
        }
        /// <summary>
        /// 返回Json字符串
        /// </summary>
        /// <param name="_msg">消息，不能为空，约定成功返回 success </param>
        /// <param name="_code">错误代码， 0 表示成功, 1 表示失败，默认为失败</param>
        /// <param name="_data">需要返回的数据，默认为 null </param>
        /// <returns></returns>
        public static string show(string _msg, int _code = 1, object _data = null)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new mkjson(_msg, _code, _data));
        }
    }

    public class Common
    {
        /// <summary>
        /// Json转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var jsonObject = (T)ser.ReadObject(ms);
            ms.Close();
            return jsonObject;
        }
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
        public void xmlSave(string path)
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
        public void xmlRead(string path)
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
        //转换节次格式
        public string AddSeparator(int classnumber)
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
        //汉字转16进制
        public string Get16binary(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            string[] strArr = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                strArr[i] = bytes[i].ToString("x");

            }
            return string.Join("", strArr);
        }
    }
}

