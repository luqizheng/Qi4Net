using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Qi.Net
{
    public class PingHost
    {
        public PingReply Ping(string hostOrIp)
        {
            Ping pingSender = new Ping();
            //Ping 选项设置  
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            //测试数据  
            string data = "test data abcabc";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            //设置超时时间  
            int timeout = 1200;
            //调用同步 send 方法发送数据,将返回结果保存至PingReply实例  
            var reply = pingSender.Send(hostOrIp, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                return reply;
                /*lst_PingResult.Items.Add("答复的主机地址：" + reply.Address.ToString());  
                lst_PingResult.Items.Add("往返时间：" + reply.RoundtripTime);  
                lst_PingResult.Items.Add("生存时间（TTL）：" + reply.Options.Ttl);  
                lst_PingResult.Items.Add("是否控制数据包的分段：" + reply.Options.DontFragment);  
                lst_PingResult.Items.Add("缓冲区大小：" + reply.Buffer.Length);  */
            }
            else
                throw new ApplicationException("Can't find the arrive at target " + hostOrIp.ToString());

        }

    }
}
