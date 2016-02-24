using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSupervisor.Models
{

    /// <summary>
    /// 课程模型，数据表classes的映射
    /// </summary>
    public class ClassesModel
    {
        public string Cid { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string ClassContent { get; set; }
        public string ClassType { get; set; }
        public string Address { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public int ClassNumber { get; set; }
        public int CheckNumber { get; set; }
    }
}
