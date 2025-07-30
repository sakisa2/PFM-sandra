using System.Text.Json.Serialization;

namespace PFM.Backend.Models.Validation
{
    public class ValidationError
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } 

        public ValidationError(string tag, string error, string message)
        {
            Tag = tag;
            Error = error;
            Message = message;
        }
    }
}
