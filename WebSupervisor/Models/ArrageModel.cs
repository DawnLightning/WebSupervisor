using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{
    public class ArrageModel
    {   /// <summary>
        ///安排编号--->cid+week+day+number
        /// </summary>
        public string Pid { set; get; }
        /// <summary>
        /// 课程编号
        /// </summary>
        public string Cid { set; get; }
        /// <summary>
        /// 督导组
        /// </summary>
        public string SuperVisors { set; get; }
        /// <summary>
        /// 确认状态(0--->未确认)
        /// </summary>
        public int Stauts { set; get; }
    }
}