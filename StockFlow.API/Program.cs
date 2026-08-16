using Microsoft.OpenApi;
using StockFlow.Application;
using StockFlow.Infrastructure;
using System.Collections.Generic;
namespace StockFlow.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.RegisterInfrastructureServices(builder.Configuration, builder.Environment.ContentRootPath);
            builder.Services.RegisterApplicationServices();
            builder.Services.RegisterAPIServices(builder.Configuration);
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "StockFlow API",
                    Version = "v1",
                    Description = "API for the StockFlow inventory management system."
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("frontend-dev", policy =>
                    policy.WithOrigins("https://localhost:7158")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                    options.RouteTemplate = "{documentName}/specifications.json");

                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "api";
                    options.SwaggerEndpoint("/v1/specifications.json", "api");
                });
            }

            app.UseCors("frontend-dev");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
