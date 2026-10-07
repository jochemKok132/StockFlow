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

            var services = new ServiceCollection();

            // Core Authentication & Storage
            services.AddAuthorizationCore();
            services.AddScoped<SessionStorageService>();
            services.AddScoped<AuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());

            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<ICashRegisterService, CashRegisterService>();

            // HTTP Client & Handler
            services.AddTransient<AuthHandler>();
            services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthHandler>();
                handler.InnerHandler = new HttpClientHandler();
                return new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7037/") };
            });

            // WinForms UI Registration
            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();

            ServiceProvider = services.BuildServiceProvider();

            // Show the login screen, then the register. After a logout, start over with a fresh scope
            // (new session, new HttpClient). Closing the login screen with Esc exits the app.
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