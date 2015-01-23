using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Qi.Domain.Attributes
{
    /// <summary>
    /// </summary>
    public class DateRangeAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage =
            "'{0}' must be a date between {1:d} and {2:d}.";

        private string _dateFormat;

        /// <summary>
        /// </summary>
        /// <param name="minDate">请输入 2014-10-01</param>
        /// <param name="maxDate">请输入2014-09-02</param>
        public DateRangeAttribute(string minDate, string maxDate)
            : this(minDate, maxDate, "yyyy-MM-dd", "yy-mm-dd")
        {
        }

        /// <summary>
        ///     初始化DateRange。请与DisplayFormat设置客户端插件接受的日期格式。
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="dateFromatForTranslate">转换格式</param>
        /// <param name="clientDateFormat">客户端的输出格式，改字符会用于客户端日期插件</param>
        public DateRangeAttribute(string minDate, string maxDate, string dateFromatForTranslate, string clientDateFormat)
            : base(DefaultErrorMessage)
        {
            if (minDate == null) throw new ArgumentNullException("minDate");
            if (maxDate == null) throw new ArgumentNullException("maxDate");
            DateFromatForTranslate = dateFromatForTranslate;
            ClientDateFormat = clientDateFormat;
            MaxDate = maxDate;
            MinDate = minDate;
        }

        /// <summary>
        /// </summary>
        public string DateFromatForTranslate
        {
            get { return _dateFormat ?? ("yyyy-MM-dd"); }
            set { _dateFormat = value; }
        }

        /// <summary>
        ///     客户端的日期格式与客户端使用的插件相关，如果使用时jqueryui，请使用jqueryui 的日期格式
        /// </summary>
        public string ClientDateFormat { get; set; }


        /// <summary>
        /// </summary>
        public string MinDate { get; set; }

        public DateTime MinDateShow
        {
            get { return ParseDate(MinDate); }
        }


        /// <summary>
        /// </summary>
        public string MaxDate { get; set; }

        public DateTime MaxDateShow
        {
            get { return ParseDate(MaxDate); }
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (!(value is DateTime))
            {
                return true;
            }
            var dateValue = (DateTime) value;
            DateTime minDate = ParseDate(MinDate);
            DateTime maxDate = ParseDate(MaxDate);
            return minDate <= dateValue && maxDate <= dateValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                ErrorMessageString,
                name, ParseDate(MinDate), ParseDate(MaxDate));
        }

        /// <summary>
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public DateTime ParseDate(string dateValue)
        {
            if (dateValue == null)
                return DateTime.MinValue;
            if (dateValue.StartsWith("+") || dateValue.StartsWith("-"))
            {
                DateTime result = DateTime.Today;
                string[] ary = dateValue.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string express in ary)
                {
                    char unit = express[express.Length - 1];

                    int exs = Convert.ToInt32(express.TrimStart('+').TrimEnd('y', 'd', 'w', 'm'));
                    switch (unit)
                    {
                        case 'y':
                            result = result.AddYears(exs);
                            break;

                        case 'd':
                            result = result.AddDays(exs);
                            break;

                        case 'w':
                            result = result.AddYears(exs);
                            break;

                        case 'm':
                            result = result.AddYears(exs);
                            break;
                    }
                }
                return result;
            }
            return DateTime.ParseExact(dateValue, DateFromatForTranslate, CultureInfo.InvariantCulture);
        }
    }
}