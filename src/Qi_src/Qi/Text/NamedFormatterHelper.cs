using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qi.Text
{
    /// <summary>
    /// </summary>
    public class NamedFormatterHelper
    {
        private const string pattern = @"\[([^\[\]]|\[([^\[\]])*\])*\]";

        /// <summary>
        ///     use replacePatten to replace Variables those in ormatString,
        ///     Variable need to defined in  square brackets, such as like that "[var]"
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="replacePattern">a dictionary with key and value,the key without "["</param>
        /// <returns></returns>
        public string Replace(string formatString, IDictionary<string, string> replacePattern)
        {
            if (replacePattern == null)
                throw new ArgumentNullException("replacePattern");
            var rex = new Regex(pattern, RegexOptions.IgnoreCase);
            return rex.Replace(formatString, match =>
                {
                    var key = match.Value.Substring(1, match.Value.Length - 2);
                    return replacePattern.ContainsKey(key) ? replacePattern[key] : match.Value;
                });
        }

        /// <summary>
        ///     Collect variable express in content, such as "I am a [varName]"
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] CollectVariable(string content)
        {
            if (content == null)
                throw new ArgumentNullException("content");


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