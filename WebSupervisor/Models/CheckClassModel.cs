using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{
    public class CheckClassModel
    {   /// <summary>
        ///教师编号 
        /// </summary>
        public string Tid { set; get; }
        /// <summary>
        /// 督导周听课
        /// </summary>
        public int WeekNumber { set; get; }
        /// <summary>
        /// 督导日听课
        /// </summary>
        public int DayNmuber { set; get; }
        /// <summary>
        /// 督导总听课
        /// </summary>
        public int total { set; get; }
    }
}