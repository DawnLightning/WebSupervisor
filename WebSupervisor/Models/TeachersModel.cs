using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSupervisor.Models
{
    public class TeachersModel
    {

        public string Tid { get; set; }
        public string TeacherName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string College { get; set; }
        public int Indentify { get; set; }
        public string Title { get; set; }
      
        public int Islimit { get; set; }
        public string Password { get; set; }
        public string TeacherRoom { set; get; }
        public string WeChatId { set; get; }
    }
}
