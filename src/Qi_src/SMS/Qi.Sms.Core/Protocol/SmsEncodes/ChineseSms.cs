namespace Qi.Sms.Protocol.SmsEncodes
{
    public abstract class ChineseSms : AbstractEncoder
    {
        protected override string FormatPhone(string phone)
        {
            phone = phone.TrimStart('+');
            if (!phone.StartsWith("86"))
                return "86" + phone;
            return phone;
        }
    }
}