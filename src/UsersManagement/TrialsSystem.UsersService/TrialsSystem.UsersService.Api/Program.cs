using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TrialsSystem.UsersService.Api.Application.Validation;
using TrialsSystem.UsersService.Api.Filters;
using TrialsSystem.UsersService.Domain.AggregatesModel.UserAggregate;
using TrialsSystem.UsersService.Infrastructure.Contexts;
using TrialsSystem.UsersService.Infrastructure.Helpers;
using TrialsSystem.UsersService.Infrastructure.Mapping;
using TrialsSystem.UsersService.Infrastructure.Repositories;

namespace TrialsSystem.UsersService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
                config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables());


            // Add services to the container.

            builder.Services.AddControllers();

            UsersServiceDAL(builder);
            builder.Services.AddScoped<IIdentityHelper, IdentityHttpRequestHelper>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "TrialsSystem.UsersService", Version = "v1" }

                    );
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
            builder.Services.AddAutoMapper(mc => mc.AddProfile(new MappingProfile()));

            builder.Services.AddScoped<UserExceptionFilter>();
            builder.Services.AddScoped<DeviceExceptionFilter>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void UsersServiceDAL(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UserContextConnection")));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();

        }
    }
}