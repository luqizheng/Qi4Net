/* code from 
 * http://www.codeproject.com/KB/cs/cs_ini.aspx 
 * By BLaZiNiX | 14 Mar 2002
 * */

using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;

namespace Qi.IO
{
    public class IniReader
    {
        public readonly string Path;

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniReader(string iniPath)
        {
            Path = iniPath;
        }

        public string[] Sections
        {
            get
            {
                //Note:have to use Bytes to read，StringBuilder only to read first Section
                var buffer = new byte[65535];
                int bufLen = 0;
                bufLen = GetPrivateProfileString(null, null, null, buffer, buffer.GetUpperBound(0), Path);
                var result = new StringCollection();
                GetStringsFromBuffer(buffer, bufLen, result);
                var stringResult = new string[result.Count];
                result.CopyTo(stringResult, 0);
                return stringResult;
            }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
                                                             string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                                                          string key, string def, StringBuilder retVal,
                                                          int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal,
                                                          int size, string filePath);

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string section, string Key)
        {
            var temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, Key, "", temp,
                                            255, Path);
            return temp.ToString();
        }

        private static void GetStringsFromBuffer(Byte[] buffer, int bufLen, StringCollection strings)
        {
            strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((buffer[i] == 0) && ((i - start) > 0))
                    {
                        string s = Encoding.GetEncoding(0).GetString(buffer, start, i - start);
                        strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
    }
}