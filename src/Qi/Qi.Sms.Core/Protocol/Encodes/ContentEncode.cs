using System;

namespace Qi.Sms.Protocol.Encodes
{
    /********************  SMS-SUBMIT-PDU  ***********************/

    /*   SCA              PDUType   MR   DA               PID   DCS    VP     UDL    UD
     * 0891683110102105F0 11        00   0B813141968548F1 00    08     10     02     0041  使用有效期格式的     
     */

    /*************************************************************/

    public class SmsInfo
    {
        public string DAAddr = "";
        public string DALen = "08";
        public string DAType = "91";
        public string DCS = "08"; // 00:7bit数据编码(默认字符集);F6:8bit数据编码; 08:USC2(16bit)双字节字符集
        public string MR = "00";
        public string OAAddr = "";

        public string OALen = "08";
        public string OAType = "91";
        public string PDUType = "39";

        public string PID = "00"; //对于标准情况下的下 MS-to-SC 短消息传送，只需设置 PID 为 00
        public string SCAAddr; //SMSC地址   8613800250500，补‘F’凑成偶数个  68 31 08 20 05 05 F0  
        public string SCALen = "08"; //SMSC地址信息的长度 共8个八位字节(包括91)  
        public string SCAType = "91"; //SMSC地址格式(TON/NPI) 用国际格式号码(在前面加‘+’)
        public string SCTS = ""; //SMSC接收到消息时的时间戳占用七个8位组，格式如 VP 的第二种情况所示,和PDUType的
        public string UD = ""; //用户数据
        public string UDL = ""; //用户数据长度
        public string VP = ""; //信息有效期

        public SmsInfo()
        {
        }

        public SmsInfo(string SCAAddr, string DAAddr, string UD)
        {
            if (SCAAddr == null) throw new ArgumentNullException("SCAAddr");
            if (DAAddr == null) throw new ArgumentNullException("DAAddr");
            if (UD == null) throw new ArgumentNullException("UD");
            this.SCAAddr = SCAAddr;
            this.DAAddr = DAAddr;
            this.UD = UD;
        }

        public SmsInfo(string SCAAddr, string PDUType, string DAAddr, string DCS, string UD)
        {
            if (SCAAddr == null) throw new ArgumentNullException("SCAAddr");
            if (PDUType == null) throw new ArgumentNullException("PDUType");
            if (DAAddr == null) throw new ArgumentNullException("DAAddr");
            if (DCS == null) throw new ArgumentNullException("DCS");
            if (UD == null) throw new ArgumentNullException("UD");
            this.SCAAddr = SCAAddr;
            this.PDUType = PDUType;
            this.DAAddr = DAAddr;
            this.DCS = DCS;
            this.UD = UD;
        }

        /// <summary>
        /// 编码要发送的内容
        /// 通过调用CodingHelper中的函数按照如下格式组成编码:SCA + PDUType + MR + DA + PID + DCS + VP + (UDL + UD)
        /// </summary>
        /// <returns>要发送的内容</returns>
        public String EncodingSMS(out int SMSLen)
        {
            int VPLen = 0;
            string SCA = CodingHelper.EncodingMobileNum(SCAAddr, SCAType, true);
            string DA = CodingHelper.EncodingMobileNum(DAAddr, DAType, false);
            string UDContent = GetUDEncoding(PDUType, DCS, UD);

            string encodeSMS = SCA + PDUType + MR + DA + PID + DCS + GetVP(PDUType, 1, out VPLen) + UDContent;

            //短信长度，即短信中心号码后的长度用十进制表示 0891683108100005F0 1100 0B 813141968548F1 00 0810020041
            // 0891683108100005F0 1000 08 91683141968548F1 00 08 A7044E2D6587
            //0891683108100005F0 24 00 08 91683141968548F1 00 08 907041619580 80 044E2D6587
            //0891683108100005F0 11 00 0D 91683141968548F1 00 08 00              044E2D6587

            SMSLen = 2 + DA.Length/2 + 2 + VPLen + UDContent.Length/2; //说明: 2（PDUType和MR的长度）+DA的长度+2（PID和DCS的长度）+VP的长度


            return encodeSMS;
        }

        /// <summary>
        /// 解码短信内容,如:
        /// 0891
        /// 683108100005F0
        ///44
        ///05
        ///A1
        ///0180F6
        ///000890709001821423
        ///8C
        ///0500030A0201
        ///8BF756DE590D5E8F53F7000A0031002E8BDD8D3979EF5206000A0032002E670065B04F1860E0000A0033002E4E1A52A1529E7406000A0034002E68A67F515B9A5236000A0035002E595799104E1A52A1000A0036002E5E3875
        ///284FE1606F000A4E2D56FD79FB52A853174EAC516C53F8002000200020002000200020621656DE59
        ///0D5B576BCD
        /// </summary>
        /// <param name="smsContent"></param>
        /// <returns></returns>
        public SmsInfo DecodingSMS(string smsContent)
        {
            var info = new SmsInfo();
            info.SCALen = smsContent.Substring(0, 2);
            info.SCAType = smsContent.Substring(2, 2);
            int SCALen = Convert.ToInt32(info.SCALen, 16);
            info.SCAAddr = CodingHelper.DecodingMobileNum(smsContent.Substring(4, (SCALen - 1)*2), info.SCAType);
            info.PDUType = smsContent.Substring((SCALen + 1)*2, 2);
            info.OALen = smsContent.Substring((SCALen + 1)*2 + 2, 2);
            int OALen = Convert.ToInt32(info.OALen, 16);
            info.OAType = smsContent.Substring((SCALen + 1)*2 + 2 + 2, 2);
            info.OAAddr = CodingHelper.DecodingMobileNum(smsContent.Substring((SCALen + 1)*2 + 2 + 2 + 2, OALen + 1),
                                                         info.OAType);
            info.PID = smsContent.Substring((SCALen + 1)*2 + 2 + 2 + 2 + (OALen + 1), 2);
            info.DCS = smsContent.Substring((SCALen + 1)*2 + 2 + 2 + 2 + (OALen + 1) + 2, 2);
            info.SCTS =
                CodingHelper.DecodingTime(smsContent.Substring((SCALen + 1)*2 + 2 + 2 + 2 + (OALen + 1) + 2 + 2, 14)).
                    ToString("yyyy-MM-dd HH:mm:ss");
            info.UDL = smsContent.Substring((SCALen + 1)*2 + 2 + 2 + 2 + (OALen + 1) + 2 + 2 + 14, 2);
            info.UD = GetUDDecoding(info.PDUType, info.DCS,
                                    smsContent.Substring((SCALen + 1)*2 + 2 + 2 + 2 + (OALen + 1) + 2 + 2 + 14 + 2));
            return info;
        }

        /// <summary>
        /// 根据PDUType确定有效期是使用相对格式还是绝对格式
        /// </summary>
        /// <param name="PDUType">PDUType串第4和3两位确定有效期的格式
        /// Bit No.  7   6   5   4   3   2   1   0
        ///          0   0   0   0   0   0   0   0
        /// VPF：    有效期格式（Validity Period Format），
        ///           00 – VP 段没有提供（长度为 0 ），
        ///           01 – 保留，        ///
        ///           10 – VP 段以整型形式提供（相对的），
        ///           11 – VP 段以8位组的一半(semi-octet)形式提供（绝对的）
        /// </param>
        /// <param name="VPDays">有效期的天数,0-7</param>
        /// <returns></returns>
        private string GetVP(string PDUType, int VPDays, out int vpLen)
        {
            string pduBitType = Convert.ToString(int.Parse(PDUType[0].ToString()), 2).PadLeft(4, '0') +
                                Convert.ToString(int.Parse(PDUType[1].ToString()), 2).PadLeft(4, '0');
            string vp = "";
            vpLen = 1;
            switch (pduBitType[3] + pduBitType[4].ToString())
            {
                case "10":
                    vpLen = 1;
                    if (VPDays <= 0)
                        vp = "00";
                    else
                    {
                        if (VPDays < 8)
                        {
                            int days = VPDays + 166;
                            vp = days.ToString("X2");
                        }
                        else
                        {
                            vp = "00";
                        }
                    }

                    break;
                case "11":
                    vpLen = 7;
                    vp = CodingHelper.EncodingTime(DateTime.Now.AddDays(VPDays));
                    break;

                default:
                    vpLen = 0;
                    vp = "";
                    break;
            }

            return vp;
        }

        /// <summary>
        /// 获取短信内容的编码 ???需要添加长短信的编码
        /// </summary>
        /// <param name="DCS"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetUDEncoding(string PDUType, string DCS, string content)
        {
            string pduBitType = Convert.ToString(int.Parse(PDUType[0].ToString()), 2).PadLeft(4, '0') +
                                Convert.ToString(int.Parse(PDUType[1].ToString()), 2).PadLeft(4, '0');
            if (pduBitType[1].ToString() == "1") //即pdu二进制传的UDHI位为1 表示是超长短信
            {
                int pduHeadLen = Convert.ToInt32(content.Substring(0, 2), 16); //pdu头部长度有的是6有的是7大部分是6，这个长度表示后面还有几个字节的头部位
                content = content.Substring(0, 2 + pduHeadLen*2);
            }
            string encodeContent = "";

            switch (DCS)
            {
                case "00":
                    encodeContent = CodingHelper.EncodingBit7(content);
                    break;

                case "F6":
                    encodeContent = CodingHelper.EncodingBit8(content);
                    break;
                case "08":
                    encodeContent = CodingHelper.EncodingUCS2(content);
                    break;
                default:
                    encodeContent = CodingHelper.EncodingUCS2(content);
                    break;
            }

            return encodeContent;
        }

        /// <summary>
        /// 对短信内容进行编码
        /// </summary>
        /// <param name="DCS"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetUDDecoding(string PDUType, string DCS, string content)
        {
            string pduBitType = Convert.ToString(int.Parse(PDUType[0].ToString()), 2).PadLeft(4, '0') +
                                Convert.ToString(int.Parse(PDUType[1].ToString()), 2).PadLeft(4, '0');
            if (pduBitType[1].ToString() == "1") //即pdu二进制传的UDHI位为1 表示是超长短信
            {
                int pduHeadLen = Convert.ToInt32(content.Substring(0, 2), 16); //pdu头部长度有的是6有的是7大部分是6，这个长度表示后面还有几个字节的头部位
                content = content.Substring(2 + pduHeadLen*2);
            }
            string encodeContent = "";

            switch (DCS)
            {
                case "00":
                    encodeContent = CodingHelper.DecodingBit7(content);
                    break;

                case "F6":
                    encodeContent = CodingHelper.DecodingBit8(content);
                    break;
                case "08":
                    encodeContent = CodingHelper.DecodingUcs2(content);
                    break;
                default:
                    encodeContent = CodingHelper.DecodingUcs2(content);
                    break;
            }

            return encodeContent;
        }
    }
}