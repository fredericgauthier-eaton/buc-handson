using System;

namespace PeopleApi.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the incoming request here if needed
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

        await _next(context);

        // Log the outgoing response here if needed
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}
