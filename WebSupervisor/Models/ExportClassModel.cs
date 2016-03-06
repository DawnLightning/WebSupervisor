using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSupervisor.Models
{
    class ExportClassModel
    {
        private string teachername;//教师姓名
        private int week;//周
                         /// <summary>
                         /// 上课周
                         /// </summary>
        public int Week
        {
            get { return week; }
            set { week = value; }
        }
        /// <summary>
        /// 星期
        /// </summary>
        private int day;//星期

        public int Day
        {
            get { return day; }
            set { day = value; }
        }
        /// <summary>
        /// 教师姓名
        /// </summary>
        public string Teachername
        {
            get { return teachername; }
            set { teachername = value; }
        }
        /// <summary>
        /// 课程类型
        /// </summary>
        private string classtype;//课程类型

        public string Classtype
        {
            get { return classtype; }
            set { classtype = value; }
        }
        private string classname;
        /// <summary>
        /// 课程名称
        /// </summary>
        public string Classname
        {
            get { return classname; }
            set { classname = value; }
        }
        private int start;//开始节次
                          /// <summary>
                          /// 上课开始的节次
                          /// </summary>
        public int Start
        {
            get { return start; }
            set { start = value; }
        }
        private int end;//终止节次
                        /// <summary>
                        /// 上课结束的节次，例如3-4节，则start为3 end为4.
                        /// </summary>
        public int End
        {
            get { return end; }
            set { end = value; }
        }
        private bool isOverTop;//是否超过2节课
                               /// <summary>
                               /// 判断是否只有2节课
                               /// </summary>
        public bool IsOverTop
        {
            get { return isOverTop; }
            set { isOverTop = value; }
        }
        public ExportClassModel()
        {

        }
        public ExportClassModel(string teachername, string classtype, int week, int day, int start, int end, bool isOverTop, string classname)
        {
            this.classtype = classtype;
            this.end = end;
            this.isOverTop = isOverTop;
            this.teachername = teachername;
            this.start = start;
            this.week = week;
            this.day = day;
            this.classname = classname;
        }
    }
}
