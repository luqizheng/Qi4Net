using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qi.Text
{
    /// <summary>
    /// 
    /// </summary>
    public class NamedFormatterHelper
    {
        /// <summary>
        /// use replacePatten to replace Variables those in <seealso cref="formatString"/>, 
        /// Variable need to defined in  square brackets, such as like that "[var]"
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="replacePattern">a dictionary with key and value,the key without "["</param>
        /// <returns></returns>
        public string Replace(string formatString, IDictionary<string, string> replacePattern)
        {
            if (replacePattern == null)
                throw new ArgumentNullException("replacePattern");

            string[] variables = CollectVariable(formatString);
            IDictionary<string, string> exist = new Dictionary<string, string>();

            foreach (string variable in variables)
            {
                if (replacePattern.ContainsKey(variable))
                {
                    exist.Add(variable, replacePattern[variable]);
                }
            }


            var pattern = new string[exist.Count];
            int index = 0;
            foreach (string key in exist.Keys)
            {
                pattern[index] = string.Format("\\[{0}\\]", key);
                index++;
            }

            var rex = new Regex(String.Join("|", pattern), RegexOptions.IgnoreCase);


            return rex.Replace(formatString,
                               match => replacePattern[match.Value.Substring(1, match.Value.Length - 2)]);
        }

        /// <summary>
        /// Collect variable express in <seealso cref="content"/>, such as "I am a [varName]"
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] CollectVariable(string content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            const string pattern = @"\[[\w*\.]*\]";


            var rex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            MatchCollection groups = rex.Matches(content);


            var result = new List<string>();

            for (int i = 0; i < groups.Count; i++)
            {
                string s = groups[i].Value;
                s = s.Substring(1, s.Length - 2);
                if (!result.Contains(s))
                {
                    result.Add(s);
                }
            }
            return result.ToArray();
        }
    }
}