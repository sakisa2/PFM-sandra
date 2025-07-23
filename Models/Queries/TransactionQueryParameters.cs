using Microsoft.AspNetCore.Mvc;
using PFM.Backend.Database.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class TransactionQueryParameters
{
    [FromQuery(Name = "kinds")]
    public List<Kind>? Kinds { get; set; }

    [FromQuery(Name = "start-date")]
    public DateTime? StartDate { get; set; }

    [FromQuery(Name = "end-date")]
    public DateTime? EndDate { get; set; }

    [FromQuery(Name = "page")]
    [DefaultValue(1)]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "page-size")]
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    [FromQuery(Name = "sort-by")]
    public string? SortBy { get; set; } = "date";

    [FromQuery(Name = "sort-order")]
    [DefaultValue("asc")]
    [RegularExpression("asc|desc")]
    public string SortOrder { get; set; } = "asc";
}
