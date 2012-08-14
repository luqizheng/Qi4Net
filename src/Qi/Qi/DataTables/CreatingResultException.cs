using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.DataTables
{
    /// <summary>
    /// 
    /// </summary>
    public class CreatingResultException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CreatingResultException(string message)
            : base(message)
        {

        }
    }
}
