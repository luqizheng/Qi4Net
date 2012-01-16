using System;

namespace Qi.Sms.Encoders
{
    public interface IEncoding
    {
        string Encode(string input);
        string Decode(String input);
    }
}