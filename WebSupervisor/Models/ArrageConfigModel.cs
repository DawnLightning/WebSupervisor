using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{
    public class ArrageConfigModel
    {   /// <summary>
        ///管理员Id 
        /// </summary>
        public int Uid { set; get; }
        public int Sday { set; get; }
        /// <summary>
        /// 排课规则名称
        /// </summary>
        public string RuleName { set; get; }
        /// <summary>
        /// 开始周
        /// </summary>
        public int Bweek { set; get; }
        /// <summary>
        /// 终止周
        /// </summary>
        public int Eweek { set; get; }
        /// <summary>
        /// 周听课(督导员一周听多少次课)
        /// </summary>
        public int WeekListen { set; get; }
        /// <summary>
        /// 日听课(督导员一天听多少次课)
        /// </summary>
        public int DayListen { set;get; }
        /// <summary>
        /// 总共的排课记录数
        /// </summary>
        public int PlanNumber { set; get; }
        /// <summary>
        /// 实验课和理论课的比例
        /// </summary>
        public int Apercent { set; get; }
        /// <summary>
        /// 督导小组最少人数
        /// </summary>
        public int MinPeople { set; get; }
        /// <summary>
        /// 督导小时最大人数
        /// </summary>
        public int MaxPeople { set; get; }
        
    }
}