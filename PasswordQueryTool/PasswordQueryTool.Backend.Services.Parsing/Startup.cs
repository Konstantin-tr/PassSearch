using GreenPipes;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Minio.AspNetCore;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Backend.Services.Parsing.Configuration;
using PasswordQueryTool.Backend.Services.Parsing.Consumers;
using PasswordQueryTool.Backend.Services.Parsing.Database;
using PasswordQueryTool.Backend.Services.Parsing.Services;
using PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces;
using PasswordQueryTool.Backend.Services.Parsing.StateMachines;
using System;

namespace PasswordQueryTool.Backend.Services.Parsing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PasswordQueryTool.Backend.Services.Parsing v1"));
            }

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appOrigin = Configuration.GetConnectionString("parsingApp");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                builder.WithOrigins(appOrigin)
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            });

            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "PasswordQueryTool.Backend.Services.Parsing", Version = "v1" }));

            services.AddScoped<IFileService, FileService>();

            var minioConfig = Configuration.GetSection("Minio").Get<MinioConfig>();

            services.AddMinio(options =>
            {
                options.Endpoint = minioConfig.Endpoint;
                options.AccessKey = minioConfig.AccessKey;
                options.SecretKey = minioConfig.SecretKey;
            });

            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddSagaStateMachine<ImportStateMachine, ImportState, ImportStateMachineDefinition>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;

                        r.AddDbContext<DbContext, ParsingDbContext>((_, optionsBuilder) => optionsBuilder.UseNpgsql(Configuration.GetConnectionString("parsingDB")));

                        r.UsePostgres();

                        r.CustomizeQuery(input => input.Include(e => e.Chunks));
                    });

                cfg.AddConsumersFromNamespaceContaining<ConsumerAnchor>();

                var rabbitConfig = Configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();

                cfg.UsingRabbitMq((context, config) =>
                {
                    config.Host(rabbitConfig.Host, rabbitConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitConfig.UserName);
                        h.Password(rabbitConfig.Password);
                    });

                    config.UseInMemoryOutbox();

                    config.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4)));
                    config.UseDelayedRedelivery(r => r.Incremental(5, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60)));

                    config.ConfigureEndpoints(context);
                });

                cfg.AddRequestClient<ISubmitImport>();
                cfg.AddRequestClient<IImportStateRequested>();
                cfg.AddRequestClient<IRequestCancellation>();
            });

            services.AddMassTransitHostedService();

            services.AddDbContext<ParsingDbContext>(config => config.UseNpgsql(Configuration.GetConnectionString("parsingDB")));

            services.AddHostedService<EfDbStartupHostedService>();
        }
    }
}