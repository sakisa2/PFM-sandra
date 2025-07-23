namespace PFM.Backend.Models.Validation
{
    public class ValidationError
    {
        public string Tag { get; set; } 
        public string Error { get; set; } 
        public string Message { get; set; } 

        public ValidationError(string tag, string error, string message)
        {
            Tag = tag;
            Error = error;
            Message = message;
        }
    }
}
