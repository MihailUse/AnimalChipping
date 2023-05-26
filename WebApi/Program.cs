using Application;
using Application.Interfaces;
using Domain;
using Domain.Persistence;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.Attributes;
using WebApi.AuthenticationHandlers;
using WebApi.Converters;
using WebApi.Services;

namespace WebApi;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services 
        builder.Services.AddDomainServices(builder.Configuration);
        builder.Services.AddInfrastructureServices();
        builder.Services.AddApplicationServices();
        builder.Services.AddScoped<ICurrentAccount, CurrentAccountService>();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ErrorHandlerAttribute>();
                options.Filters.Add<ValidateIdentifierAttribute>();
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Scheme = "Basic",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme.",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
                }
            };

            options.AddSecurityDefinition("Basic", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    securityScheme,
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddAuthentication("Basic")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

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

        app.MapControllers();
        app.Run();
    }
}