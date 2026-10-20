using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using StockFlow.API;

namespace StockFlow.UnitTests.Api
{
    public class ViewOnlyGuardTests
    {
        private static (ViewOnlyGuard guard, Func<bool> nextCalled) CreateSut()
        {
            bool called = false;
            var guard = new ViewOnlyGuard(_ => { called = true; return Task.CompletedTask; });
            return (guard, () => called);
        }

        private static DefaultHttpContext Request(string method, string path, string? role)
        {
            var claims = role is null ? Array.Empty<Claim>() : [new Claim(ClaimTypes.Role, role)];
            var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) };
            context.Request.Method = method;
            context.Request.Path = path;
            context.Response.Body = new MemoryStream();
            return context;
        }

        [Theory]
        [InlineData("POST", "/Product/Create")]
        [InlineData("PUT", "/Product/Update")]
        [InlineData("DELETE", "/Product/SoftDelete/1")]
        [InlineData("POST", "/CashRegister/Purchase")]
        public async Task InvokeAsync_TesterChanges_Returns403AndSkipsEndpoint(string method, string path)
        {
            var (sut, nextCalled) = CreateSut();
            var context = Request(method, path, AccessRules.ViewOnlyRole);

            await sut.InvokeAsync(context);

            Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
            Assert.False(nextCalled());
        }

        [Fact]
        public async Task InvokeAsync_TesterReads_CallsEndpoint()
        {
            var (sut, nextCalled) = CreateSut();
            var context = Request("GET", "/Product/GetAll", AccessRules.ViewOnlyRole);

            await sut.InvokeAsync(context);

            Assert.True(nextCalled());
        }

        [Fact]
        public async Task InvokeAsync_TesterLogsIn_CallsEndpoint()
        {
            var (sut, nextCalled) = CreateSut();
            var context = Request("POST", "/Employee/Login", AccessRules.ViewOnlyRole);

            await sut.InvokeAsync(context);

            Assert.True(nextCalled());
        }

        [Fact]
        public async Task InvokeAsync_ManagerChanges_CallsEndpoint()
        {
            var (sut, nextCalled) = CreateSut();
            var context = Request("POST", "/Product/Create", "Manager");

            await sut.InvokeAsync(context);

            Assert.True(nextCalled());
        }
    }
}
