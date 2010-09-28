using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes
{
    public interface ISmsProvider
    {
        void Send(string mobile, string content, SmsFormat type);

    }


}
