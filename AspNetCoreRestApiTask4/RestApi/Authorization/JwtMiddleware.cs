namespace RestApi.Authorization;

using RestApi.DataAccess.Users;
using RestApi.DataAccess.BlackListedTokens;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context, 
        IUserDataAccessProvider userService, 
        IJwtUtils jwtUtils, 
        BlackListedTokenPostgreSqlContext blackListedTokenContext)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateToken(token);

        var blackListedToken = blackListedTokenContext.BlackListedTokens.ToList().Find(t => token == t.Token);

        if (blackListedToken != null)
        {
            userId = null;
        }
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = userService.GetById(userId.Value);
        }

        await _next(context);
    }
}