﻿using RestaurantAPI.Exceptions;
using RestaurantAPI.Exeptions;

namespace RestaurantAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> loger)
        {
            _logger = loger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (ForbidException forbidException)
            {
                context.Response.StatusCode = 403;
            }
            catch (BadRequestException badRequestException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badRequestException.Message);
            }
            catch(NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");

            }
        }
    }
}
