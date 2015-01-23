using System.ComponentModel.DataAnnotations;

namespace Qi.Domain.Attributes
{
    /// <summary>
    /// </summary>
    public class QiRangeAttribute : RangeAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        public QiRangeAttribute(double min, double max, double step) : base(min, max)
        {
            Step = step;
        }

        /// <summary>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        public QiRangeAttribute(int min, int max, int step) : base(min, max)
        {
            Step = step;
        }

        /// <summary>
        /// </summary>
        public object Step { get; set; }
    }
}