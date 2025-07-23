using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AddCsvRequestBodyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody == null &&
            context.ApiDescription.HttpMethod?.ToUpper() == "POST" &&
            context.ApiDescription.RelativePath?.ToLower().Contains("import") == true)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = false,
                Content = {
                    ["application/csv"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    }
                }
            };
        }
    }
}