using StockFlow.Shared.Common;

namespace StockFlow.API
{
    public class ViewOnlyGuard(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.IsInRole(AccessRules.ViewOnlyRole) &&
                !AccessRules.IsReadOnlyMethod(context.Request.Method) &&
                !IsLogin(context.Request.Path))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(AccessRules.ViewOnlyMessage));
                return;
            }

            await next(context);
        }

        private static bool IsLogin(PathString path) =>
            path.Value?.EndsWith("/Login", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
