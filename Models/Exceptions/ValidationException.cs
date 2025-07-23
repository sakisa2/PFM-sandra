using System;
using PFM.Backend.Models.Validation;

namespace PFM.Backend.Models.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; }

        public ValidationException(ValidationResult validationResult)
            : base("Validation failed")
        {
            ValidationResult = validationResult;
        }
    }
}
