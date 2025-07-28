using System;
using System.Collections.Generic;
using PFM.Backend.Models.Validation;

namespace PFM.Backend.Models.Exceptions
{
    public class BusinessException : Exception
    {
        public List<ValidationError> Errors { get; }

        public BusinessException(List<ValidationError> errors)
            : base("Business rule violation")
        {
            Errors = errors;
        }

        public BusinessException(string tag, string error, string message)
            : this(new List<ValidationError> { new ValidationError(tag, error, message) }) { }
    }
}
