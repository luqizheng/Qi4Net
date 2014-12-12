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
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="format"></param>
        public DateRangeAttribute(string minDate, string maxDate, string format)
            : base(DefaultErrorMessage)
        {
            DateFormat = format;
            MinDate = ParseDate(minDate);
            MaxDate = ParseDate(maxDate);
        }

        /// <summary>
        /// </summary>
        public string DateFormat
        {
            get { return _dateFormat ?? ("yyyy/MM/dd"); }
            set { _dateFormat = value; }
        }

        /// <summary>
        /// </summary>
        public DateTime MinDate { get; set; }

        /// <summary>
        /// </summary>
        public DateTime MaxDate { get; set; }

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
            return MinDate <= dateValue && dateValue <= MaxDate;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                ErrorMessageString,
                name, MinDate, MaxDate);
        }

        /// <summary>
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        private DateTime ParseDate(string dateValue)
        {
            return DateTime.ParseExact(dateValue, DateFormat,
                CultureInfo.InvariantCulture);
        }
    }
}