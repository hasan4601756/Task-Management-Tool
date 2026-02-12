using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TaskManagementTool.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                await _next(context);
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "SQL exception for UserId {UserId}, Path {Path}, TraceId {TraceId}",
                    userId ?? "Anonymous",
                    context.Request.Path,
                    context.TraceIdentifier);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Database error",
                    Detail = "A database error occurred while processing your request."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception for UserId {UserId}, Path {Path}, TraceId {TraceId}",
                    userId ?? "Anonymous",
                    context.Request.Path,
                    context.TraceIdentifier);


                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unexpected error",
                    Detail = "An unexpected error occured."
                });
            }
        }
    }
}