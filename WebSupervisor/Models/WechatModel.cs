using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSupervisor.Models
{
    //发送安排模型
    class ArrageMessagesModel
    {
        public List<ArrageMessage> ArrageMessages;
    }
    class ArrageMessage
    {
        public List<string> Phone;
        public string Message;
    }
}
