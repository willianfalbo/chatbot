using System;
using AutoMapper;
using AutoMapper.Configuration;
using Chatbot.API;
using Chatbot.API.Helpers;
using Chatbot.API.Mappings;
using Chatbot.Common.Interfaces;
using Chatbot.Manager;
using Chatbot.Manager.Mappings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        private const string ALLOW_ANY_ORIGIN_POLICY = "ALLOW_ANY_ORIGIN_POLICY";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

        private static void CORSPolicySetup(IServiceCollection services)
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

        private static void AutoMapperSetup(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var config = new MapperConfigurationExpression();
            config.AddProfile<ManagerMappingProfile>();
            config.AddProfile<DTOMappingProfile>();

            Mapper.Initialize(config);

            Mapper.AssertConfigurationIsValid();
        }

        private static void ChatbotDISetup(IServiceCollection services)
        {
            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter with error handling enabled. 
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.) 
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            services.AddSingleton<MainDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
        }

        private static void ApplicationDISetup(IServiceCollection services)
        {
            // custom classes
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<ICompanyRegistryManager, CompanyRegistryManager>();
            services.AddSingleton<IAzureChannelManager, AzureChannelManager>();
            services.AddSingleton<IUserConversationManager, UserConversationManager>();
            services.AddSingleton<IDialogHelper, DialogHelper>();
        }
    }
}
