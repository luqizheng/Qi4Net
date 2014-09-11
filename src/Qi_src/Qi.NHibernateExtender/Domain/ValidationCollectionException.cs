using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Qi.Domain
{
    [System.Serializable]
    public class ValidationCollectionException : ApplicationException
    {
        private readonly ICollection<ValidationResult> _erroResults;

        public ValidationCollectionException(ICollection<ValidationResult> erroResults)
        {
            _erroResults = erroResults;
        }

        public override string Message
        {
            get
            {
                var stringBuild = new StringBuilder();
                foreach (var a in _erroResults)
                {
                    stringBuild.Append(a.MemberNames + " " + a.ErrorMessage+"\r\n");
                }
                return stringBuild.ToString();
            }
        }
    }
}
