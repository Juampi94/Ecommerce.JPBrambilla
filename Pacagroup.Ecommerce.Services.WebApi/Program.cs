using AutoMapper;
using Microsoft.Extensions.Logging;
using Pacagroup.Ecommerce.Transversal.Mapper;
using Pacagroup.Ecommerce.Transversal.Common;
using Pacagroup.Ecommerce.Infraestructure.Data;
using Pacagroup.Ecommerce.Infraestructure.Interface;
using Pacagroup.Ecommerce.Infraestructure.Repository;
using Pacagroup.Ecommerce.Domain.Interface;
using Pacagroup.Ecommerce.Domain.Core;
using Pacagroup.Ecommerce.Application.Interface;
using Pacagroup.Ecommerce.Application.Main;
using Pacagroup.Ecommerce.Services.WebApi.Helpers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pacagroup.Ecommerce.Transversal.Logging;
using System.Reflection;

internal class Program
{

    static private readonly string _myPolicy = "policyApiEcommerce";

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Pacagroup Technology Services API Market",
                Description = "A simple example ASP.NET Core Web API",
                TermsOfService = new Uri("https://pacagroup.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Alex Espejo",
                    Email = "alex.espejo.c@gmail.com",
                    Url = new Uri("https://pacagroup.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Use under LICX",
                    Url = new Uri("https://pacagroup.com/licence")
                }
            });
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Authorization by API key.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
        });

        builder.Services.AddAutoMapper(x => x.AddProfile(new MappingsProfile()));
        //builder.Services.AddMvc().AddJsonOptions();
        var appSettingsSection = builder.Configuration.GetSection("Config");
        builder.Services.Configure<AppSettings>(appSettingsSection);
        //Se declaran Singleton porque se asigna el recurso al crear, luego por petición usa la misma instancia
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();

        var appSettings = appSettingsSection.Get<AppSettings>();

        //Se declaran Scoped, se instancian una vez por solicitud
        builder.Services.AddScoped<ICustomersApplication, CustomersApplication>();
        builder.Services.AddScoped<ICustomersDomain, CustomersDomain>();
        builder.Services.AddScoped<ICustomersRepository, CustomersRepository>();
        builder.Services.AddScoped<IUsersApplication, UsersApplication>();
        builder.Services.AddScoped<IUsersDomain, UsersDomain>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        var Key = Encoding.ASCII.GetBytes(appSettings.Secret);
        var Issuer = appSettings.Issuer;
        var Audience = appSettings.Audience;

        builder.Services.AddAuthentication(a =>
        {
            a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwt =>
        {
            jwt.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var userId = int.Parse(context.Principal.Identity.Name);
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
            jwt.RequireHttpsMetadata = false;
            jwt.SaveToken = false;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        var configCorsOrigin = builder.Configuration["Config:OriginCors"];
        builder.Services.AddCors(
            options => options.AddPolicy(_myPolicy, builder => builder
            .WithOrigins(configCorsOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()));

        var app = builder.Build();

        //if (!app.Environment.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI();
        //}

        app.UseAuthorization();

        app.UseCors(_myPolicy);

        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }

}