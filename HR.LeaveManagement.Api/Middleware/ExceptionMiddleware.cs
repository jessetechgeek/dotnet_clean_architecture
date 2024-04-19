using HR.LeaveManagement.Api.Models;
using HR.LeaveManagement.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HR.LeaveManagement.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            CustomProblemDetails problem = new();

            switch (ex)
            {
                case BadRequestException badRequestException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = badRequestException.Message,
                        Status = (int)httpStatusCode,
                        Detail = badRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                        Errors = badRequestException.ValidationErrors
                    };
                    break;
                case NotFoundException notFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    problem = new CustomProblemDetails
                    {
                        Title = notFoundException.Message,
                        Status = (int)httpStatusCode,
                        Detail = notFoundException.InnerException?.Message,
                        Type = nameof(NotFoundException)
                    };
                    break;
                default:
                    problem = new CustomProblemDetails
                    {
                        Title = ex.Message,
                        Status = (int)httpStatusCode,
                        Detail = ex.StackTrace,
                        Type = nameof(Exception)
                    };
                    break;
            }
            httpContext.Response.StatusCode = (int)httpStatusCode;
            await httpContext.Response.WriteAsJsonAsync(problem);
        }
    }
}
