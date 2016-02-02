using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{
    public class AdminModel
    {
        public int UserId{ set; get; }

        public string UserName { set; get; }

        public string Password { set; get; }
        public string College { set; get; }
        public int NumSMS { set; get; }

        public int Power { set; get; }
        public AdminModel()
        {

        }


    }
}