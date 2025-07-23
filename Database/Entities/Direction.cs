using System.Text.Json.Serialization;

namespace PFM.Backend.Database.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Direction
    {
        d, // debit
        c  // credit
    }
}