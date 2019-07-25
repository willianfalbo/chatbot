using System;
using AutoMapper;
using Chatbot.Api.Adapters;
using Chatbot.Api.Bots;
using Chatbot.Api.Dialogs;
using Chatbot.Api.Helpers;
using Chatbot.Api.Mappings;
using Chatbot.Common.Configuration;
using Chatbot.Common.Interfaces;
using Chatbot.Manager;
using Chatbot.Manager.Mappings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatbot.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private const string ALLOW_ANY_ORIGIN_POLICY = "ALLOW_ANY_ORIGIN_POLICY";

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;

            _appSettings = MapConfiguration(); // load settings
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = MapConfiguration(); // load settings
            services.AddSingleton(appSettings);

            // it should be at the top of the method
            CORSPolicySetup(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            AutoMapperSetup(services);

            ChatbotDISetup(services);

            ApplicationDISetup(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // it should be at the top of the method
            app.UseCors(ALLOW_ANY_ORIGIN_POLICY);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void CORSPolicySetup(IServiceCollection services)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.2
            // adds cross-origin resource sharing these service with other apps
            services.AddCors(options =>
            {
                options.AddPolicy(ALLOW_ANY_ORIGIN_POLICY,
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        private void AutoMapperSetup(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var config = new AutoMapper.Configuration.MapperConfigurationExpression();
            config.AddProfile<ManagerMappingProfile>();
            config.AddProfile<DTOMappingProfile>();

            Mapper.Initialize(config);

            Mapper.AssertConfigurationIsValid();
        }

        private void ChatbotDISetup(IServiceCollection services)
        {
            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter with error handling enabled. 
            services.AddSingleton<IBotFrameworkHttpAdapter, DefaultAdapter>();

            CosmosDbSetup(services);

            services.AddSingleton<MainDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
        }

        private void ApplicationDISetup(IServiceCollection services)
        {
            // custom classes
            services.AddSingleton<ICompanyRegistryManager, CompanyRegistryManager>();
            services.AddSingleton<IAzureChannelManager, AzureChannelManager>();
            services.AddSingleton<IUserConversationManager, UserConversationManager>();
            services.AddSingleton<IDialogHelper, DialogHelper>();
        }

        private AppSettings MapConfiguration()
        {
            var appSettings = new AppSettings();
            _configuration.Bind(appSettings);
            return appSettings;
        }

        // https://github.com/microsoft/botframework-solutions/blob/master/templates/Virtual-Assistant-Template/csharp/Sample/VirtualAssistantSample/Startup.cs
        private void CosmosDbSetup(IServiceCollection services)
        {
            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.) 
            // services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            // services.AddSingleton<UserState>();
            // Create the Conversation state. (Used by the Dialog system itself.)
            // services.AddSingleton<ConversationState>();

            IStorage dataStore =
            new CosmosDbStorage(new CosmosDbStorageOptions
            {
                CosmosDBEndpoint = this._appSettings.CosmosDb.CosmosDBEndpoint ?? throw new Exception($"Key '{nameof(this._appSettings.CosmosDb.CosmosDBEndpoint)}' not found."),
                AuthKey = this._appSettings.CosmosDb.AuthKey ?? throw new Exception($"Key '{nameof(this._appSettings.CosmosDb.AuthKey)}' not found."),
                DatabaseId = this._appSettings.CosmosDb.DatabaseId ?? throw new Exception($"Key '{nameof(this._appSettings.CosmosDb.DatabaseId)}' not found."),
                CollectionId = this._appSettings.CosmosDb.CollectionId ?? throw new Exception($"Key '{nameof(this._appSettings.CosmosDb.CollectionId)}' not found."),
                PartitionKey = this._appSettings.CosmosDb.PartitionKey ?? throw new Exception($"Key '{nameof(this._appSettings.CosmosDb.PartitionKey)}' not found."),
            });

            // Configure storage
            // Uncomment the following line for local development without Cosmos Db
            // services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<IStorage>(dataStore);
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();
            services.AddSingleton(sp =>
            {
                var userState = sp.GetService<UserState>();
                var conversationState = sp.GetService<ConversationState>();
                return new BotStateSet(userState, conversationState);
            });
        }
    }
}
