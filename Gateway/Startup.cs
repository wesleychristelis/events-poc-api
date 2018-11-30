using EventualityPOCApi.Channel;
using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Framework;
using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Gateway.Component.PersonProfileContext;
using EventualityPOCApi.Gateway.Configuration;
using EventualityPOCApi.Gateway.TransportAdapter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EventualityPOCApi.Gateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        #region Constructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region Public
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();

            services.AddSingleton<HubPublisherWebsocket>();

            AddConfigurationAndGatewayServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            loggerFactory.AddDebug();

            var websocketConfiguration = serviceProvider.GetService<WebsocketConfiguration>();
            app.UseCors(builder => builder.WithOrigins(websocketConfiguration.AllowUrls).AllowAnyHeader().WithMethods("GET", "POST").AllowCredentials());

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSignalR(routes => routes.MapHub<HubSubscriberWebsocket>("/eventHub"));

            app.UseMvc();

            ConfigureGatewayServices(serviceProvider);
        }
        #endregion

        #region Private
        private void AddConfigurationAndGatewayServices(IServiceCollection services)
        {
            var cosmosDBConfiguration = new CosmosDBConfiguration();
            Configuration.Bind("CosmosDB", cosmosDBConfiguration);
            services.AddSingleton(cosmosDBConfiguration);

            var eventGridConfiguration = new EventGridConfiguration();
            Configuration.Bind("EventGrid", eventGridConfiguration);
            services.AddSingleton(eventGridConfiguration);

            var websocketConfiguration = new WebsocketConfiguration();
            Configuration.Bind("Websocket", websocketConfiguration);
            services.AddSingleton(websocketConfiguration);

            services.AddSingleton<IDecisionChannel, DecisionChannelRx>();
            services.AddSingleton<IPerceptionChannel, PerceptionChannelRx>();

            if (eventGridConfiguration.Enabled)
            {
                services.AddSingleton(s => new EventGridClient(new TopicCredentials(eventGridConfiguration.PersonProfileContextPerceptionTopicKey)));
                services.AddSingleton<PerceptionChannelAdapterEventGrid>();
            }
            else
            {
                // Individual components and their dependencies, seemlessly replaced by Azure functions in the cloud
                services.AddSingleton(s => new DocumentClient(new Uri(cosmosDBConfiguration.AccountEndpoint), cosmosDBConfiguration.AccountKey));
                services.AddSingleton<PersonComponent>();
                services.AddSingleton<IPersonRepository, PersonRepositoryCosmosDb>();
            }
        }

        private void ConfigureGatewayServices(IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<HubPublisherWebsocket>().RegisterOutgoingHandler();

            if (serviceProvider.GetService<EventGridConfiguration>().Enabled)
            {
                // Bind event grid adapter
                serviceProvider.GetService<PerceptionChannelAdapterEventGrid>().Configure(serviceProvider.GetService<IPerceptionChannel>());
            } else
            {
                // Bind components
                serviceProvider.GetService<PersonComponent>().Configure();

                // Initialize repositories - TODO think about a better place for this
                serviceProvider.GetService<IPersonRepository>().InitializeAsync();
            }
        }
        #endregion
    }
}
