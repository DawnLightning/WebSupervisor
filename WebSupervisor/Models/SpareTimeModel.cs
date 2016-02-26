using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{
    public class SpareTimeModel
    {   /// <summary>
        /// 教师编号
        /// </summary>
        public string Tid { set; get; }
        /// <summary>
        /// 空闲周
        /// </summary>
        public int Week { set; get; }
        /// <summary>
        /// 空闲天
        /// </summary>
        public int Day { set; get; }
        /// <summary>
        /// 空闲节次
        /// </summary>
        public int ClassNumber { set; get; }
        /// <summary>
        /// 是否被安排(0->未安排)
        /// </summary>
        public int Assign { set; get; }
    }   
}