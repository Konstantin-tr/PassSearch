using Elasticsearch.Net;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nest;
using PasswordQueryTool.Backend.Database;
using PasswordQueryTool.Backend.Database.ElasticSearch;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Backend.Services.DatabaseAccess.Configuration;
using PasswordQueryTool.Backend.Services.DatabaseAccess.Consumers;
using System;
using System.Linq;

namespace PasswordQueryTool.Backend.Services.DatabaseAccess
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDatabaseHelper databaseHelper)
        {
            //app.ApplicationServices.GetRequiredService<IDatabaseHelper>().EmptyDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PasswordQueryTool.Backend.Services.DatabaseAccess v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            databaseHelper.Setup();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "PasswordQueryTool.Backend.Services.DatabaseAccess", Version = "v1" }));



            //var elasticConfig = Configuration.GetSection("ElasticSearch").Get<ElasticSearchConfig>();
            //var uris = elasticConfig.ClusterEndpoints.Select(endpoints => new Uri(endpoints)).ToArray();

            var uris = new[]
            {
                new Uri("http://es01:9200"),
                new Uri("http://es02:9200"),
            };

            var connectionPool = new StaticConnectionPool(uris);

            var settings = new ConnectionSettings(connectionPool).DefaultIndex("passwords");

            var client = new ElasticClient(settings);

            new ElasticDatabaseHelperService(client).Setup();

            services.AddSingleton<IElasticClient>(client);

            services.AddSingleton<IDatabaseHelper, ElasticDatabaseHelperService>();

            var rabbitConfig = Configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();

            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddRequestClient<IChunkImportResultReceived>();

                cfg.AddConsumer<PasswordCombinationsConsumer>(cconfig => 
                {
                    cconfig.UseConcurrencyLimit(5);
                });

                cfg.UsingRabbitMq((context, config) =>
                {
                    config.Host(rabbitConfig.Host, rabbitConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitConfig.UserName);
                        h.Password(rabbitConfig.Password);
                    });

                    config.ConfigureEndpoints(context);

                    config.UseInMemoryOutbox();

                    config.UseDelayedRedelivery(r => r.Incremental(5, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60)));
                    config.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4)));
                });
            });

            services.AddMassTransitHostedService();
        }
    }
}