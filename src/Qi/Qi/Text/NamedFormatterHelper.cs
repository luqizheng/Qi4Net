using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qi.Text
{
    public static class NamedFormatterHelper
    {
        /// <summary>
        /// use replacePatten to replace Variables those in <seealso cref="formatString"/>, 
        /// Variable need to defined in  square brackets, such as like that "[var]"
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="replacePattern"></param>
        /// <returns></returns>
        public static string Replace(string formatString, IDictionary<string, string> replacePattern)
        {
            if (replacePattern == null)
                throw new ArgumentNullException("replacePattern");
            var pattern = new string[replacePattern.Count];
            int index = 0;
            foreach (string key in replacePattern.Keys)
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