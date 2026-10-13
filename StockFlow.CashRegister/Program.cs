using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using StockFlow.CashRegister.Authentication;
using StockFlow.CashRegister.Interfaces;
using StockFlow.CashRegister.Screens;
using StockFlow.CashRegister.Services;

namespace StockFlow.CashRegister
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var settings = RegisterSettings.Load();
            var services = new ServiceCollection();

            services.AddAuthorizationCore();
            services.AddScoped<SessionStorageService>();
            services.AddScoped<AuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<ICashRegisterService, CashRegisterService>();

            services.AddTransient<AuthHandler>();
            services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthHandler>();
                handler.InnerHandler = new HttpClientHandler();
                return new HttpClient(handler) { BaseAddress = settings.ApiBaseUri };
            });

            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();

            ServiceProvider = services.BuildServiceProvider();

            bool loggedOut;
            do
            {
                loggedOut = false;

                using var scope = ServiceProvider.CreateScope();

                var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
                if (loginForm.ShowDialog() != DialogResult.OK)
                    break;

                var mainForm = scope.ServiceProvider.GetRequiredService<MainForm>();
                System.Windows.Forms.Application.Run(mainForm);

                loggedOut = mainForm.LoggedOut;
            }
            while (loggedOut);

            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}