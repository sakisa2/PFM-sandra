using System.Collections.Generic;
using System.Linq;

namespace PFM.Backend.Models.Validation
{
    public class ValidationResult
    {
        public List<ValidationError> Errors { get; } = new();

        public bool IsValid => Errors.Count == 0;

        public void AddError(string tag, string error, string message)
        {
            Errors.Add(new ValidationError(tag, error, message));
        }
    }
}
