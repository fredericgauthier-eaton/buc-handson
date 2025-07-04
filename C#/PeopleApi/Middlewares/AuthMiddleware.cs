using System;

namespace PeopleApi.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var username = context.Request.Headers["username"].FirstOrDefault();
        var password = context.Request.Headers["password"].FirstOrDefault();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Username or password can't be blank!" });
            return;
        }
        if (username != "calaca" || password != "12345")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid credentials!" });
            return;
        }
        await _next(context);
    }
}
