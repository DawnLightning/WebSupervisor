using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{

    /// <summary>
    /// 管理员登录模型
    /// </summary>
    public class AdminModel
    {
        public int UId{ set; get; }

        public string UserName { set; get; }

        public string Password { set; get; }
        public string College { set; get; }
        public int NumSMS { set; get; }

        public int Power { set; get; }
    }
    /// <summary>
    /// 督导老师登录模型
    /// </summary>
    public class SupervisorModle
    {
    
        //public int indentity { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

    }
}