using System.Security.Claims;

namespace Ghost.Middleware;

public class AdminAuthMiddleware
{
    private readonly RequestDelegate _next;
    
    public AdminAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        
        if (context.Request.Path.StartsWithSegments("/api/admin"))
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            
            var isAdmin = user.FindFirst("IsAdmin")?.Value == "True";
            
            if (!isAdmin)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden");
                return;
            }
        }
        
        await _next(context);
    }
}