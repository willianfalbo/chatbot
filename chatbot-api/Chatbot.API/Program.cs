using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Chatbot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ChangeCurrentCulture();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((logging) =>
                {
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .UseStartup<Startup>();

        private static void ChangeCurrentCulture()
        {
            var current = Thread.CurrentThread.CurrentUICulture;

            // change the current culture if the language is not Portuguese-Brazil
            if (current.Name != "pt-BR")
            {
                var ptBrCulture = CultureInfo.CreateSpecificCulture("pt-BR");
                CultureInfo.CurrentCulture = ptBrCulture;
                Thread.CurrentThread.CurrentUICulture = ptBrCulture;
                // Make current UI culture consistent with current culture.
                Thread.CurrentThread.CurrentCulture = ptBrCulture;
            }

            // Console.WriteLine($"CULTURE AFTER: {Thread.CurrentThread.CurrentUICulture}");
            // Console.WriteLine($"CULTURE UI AFTER: {Thread.CurrentThread.CurrentCulture}");
            // Console.WriteLine($"CURRENT CULTURE: {CultureInfo.CurrentCulture}");
        }
    }
}