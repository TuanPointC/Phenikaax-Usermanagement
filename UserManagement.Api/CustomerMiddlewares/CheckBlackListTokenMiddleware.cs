using System.Net;
using Microsoft.AspNetCore.Authentication;
using StackExchange.Redis;
using UserManagement.Api.Services;

namespace UserManagement.Api.CustomerMiddlewares;

public class CheckBlackListTokenMiddleware
{
    private readonly RequestDelegate _next;

    public CheckBlackListTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context,ITokenService tokenService)
    {
        var accessToken = await context.GetTokenAsync("access_token");
        if (!string.IsNullOrEmpty(accessToken))
        {
            var res = await tokenService.CheckBlackListAccessTokenAsync(accessToken);
            if (res)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
        await _next(context);
    }
}
