using Microsoft.AspNetCore.Http;
using PFM.Backend.Models.Exceptions;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PFM.Backend.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); 
            }
            catch (ValidationException vex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var errorResponse = new { errors = vex.ValidationResult.Errors };
                var json = JsonSerializer.Serialize(errorResponse);

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>> EXCEPTION: " + ex.Message);
                Console.WriteLine(">>> STACKTRACE: " + ex.StackTrace);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    errors = new[]
                    {
                        new
                        {
                            tag = "server",
                            error = "unexpected-error",
                            message = "An unexpected error occurred"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
