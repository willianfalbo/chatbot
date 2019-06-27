using System;
using AutoMapper;
using AutoMapper.Configuration;
using Chatbot.API;
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
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //AUTO MAPPER
            AutoMapperSetup(services);

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

            // custom classes
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<ICompanyRegistryManager, CompanyRegistryManager>();
            services.AddSingleton<IDirectLineManager, DirectLineManager>();
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

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        private static void AutoMapperSetup(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var cfg = new MapperConfigurationExpression();
            cfg.AddProfile<CompanyRegistryMappingProfile>();
            cfg.AddProfile<DirectLineTokenMappingProfile>();

            Mapper.Initialize(cfg);

            Mapper.AssertConfigurationIsValid();
        }
    }
}
