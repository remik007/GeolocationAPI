using Geolocation.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Geolocation.API.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(SqliteException sqliteException)
            {
                _logger.LogError(sqliteException, sqliteException.Message);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Service Unavailable. Please try again later.",
                    Status = (int)StatusCodes.Status503ServiceUnavailable,
                    Instance = context.Request.Path,
                    Detail = sqliteException.Message,
                };
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (TimeoutException sqliteException)
            {
                _logger.LogError(sqliteException, sqliteException.Message);
                context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                context.Response.ContentType = "application/json";
                var problemDetails = new ProblemDetails
                {
                    Title = "The operation has timed out.",
                    Status = (int)StatusCodes.Status408RequestTimeout,
                    Instance = context.Request.Path,
                    Detail = sqliteException.Message,
                };
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (BadRequestException badRequestException)
            {
                _logger.LogError(badRequestException, badRequestException.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = (int)StatusCodes.Status400BadRequest,
                    Instance = context.Request.Path,
                    Detail = badRequestException.Message,
                };
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (NotFoundException notFoundException)
            {
                _logger.LogError(notFoundException, notFoundException.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Not Found",
                    Status = (int)StatusCodes.Status404NotFound,
                    Instance = context.Request.Path,
                    Detail = notFoundException.Message,
                };
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var problemDetails = new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = (int)StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path,
                    Detail = "Internal server error occured",
                };
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
