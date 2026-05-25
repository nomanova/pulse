using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ErrorOr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pulse.Api.Shared;
using Pulse.Api.Shared.Contract;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Exceptions;

namespace Pulse.Api.Common;

public static class ExceptionHandlerMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }

    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IEnvironmentProvider _environment;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IEnvironmentProvider environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ForbiddenException:
                        Response(context, HttpStatusCode.Forbidden);
                        break;
                    case UnauthorizedException:
                        Response(context, HttpStatusCode.Unauthorized);
                        break;
                    case DomainException exception:
                        await ValidationResponse(context, exception);
                        break;
                    default:
                        await UnexpectedResponse(context, ex);
                        break;
                }
            }
        }

        private static async Task ValidationResponse(HttpContext context, DomainException exception)
        {
            var errors = exception.Errors;
            var problem = errors.AsProblem();

            await ProblemResponse(context, HttpStatusCode.BadRequest, problem);
        }

        private async Task UnexpectedResponse(HttpContext context, Exception ex)
        {
            _logger.LogCritical(ex, "{Message}", ex.Message);
            await ProblemResponse(context, HttpStatusCode.InternalServerError, Error.Unexpected(), ex);
        }

        private async Task ProblemResponse(
            HttpContext context, HttpStatusCode statusCode, Error error, Exception? exception = null)
        {
            // Do not print stack traces in hosted environments
            var problem = _environment.IsVirtual ? error.AsProblem(ex: exception) : error.AsProblem();
            await ProblemResponse(context, statusCode, problem);
        }

        private static async Task ProblemResponse(HttpContext context, HttpStatusCode statusCode, Problem problem)
        {
            var json = JsonSerializer.Serialize(problem, JsonSettings.Get());
            var response = context.Response;

            response.StatusCode = (int)statusCode;
            response.ContentType = "application/problem+json";

            await response.WriteAsync(json);
        }

        private static void Response(HttpContext context, HttpStatusCode statusCode)
        {
            var response = context.Response;
            response.StatusCode = (int)statusCode;
        }
    }
}