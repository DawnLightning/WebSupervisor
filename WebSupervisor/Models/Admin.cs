using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSupervisor.Models
{
    public class Admin
    {
        public int uid { set; get; }

        public string username { set; get; }

        public string password { set; get; }
        public string college { set; get; }
        public int numsms { set; get; }

        public int power { set; get; }
        public Admin()
        {

        }


    }
}