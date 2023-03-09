using Application;
using Application.Interfaces;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.AuthenticationHandlers;
using WebApi.Converters;
using WebApi.Middlewares;
using WebApi.Services;

namespace WebApi;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddScoped<ICurrentAccount, CurrentAccountService>();

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Scheme = "Basic",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Basic"
                        }
                    },
                    new string[] { }
                }
            });
        });

        builder.Services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Database.Migrate();
        }

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseErrorHandlerMiddleware();
        app.UseAuthenticateMiddleware();

        app.MapControllers();
        app.Run();
    }
}