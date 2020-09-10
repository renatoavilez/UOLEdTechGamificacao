using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.Core.Interfaces;
using Uol.EdTech.Gamification.Core.Servicos;

namespace Uol.EdTech.Gamification.WorkerExecutor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Information("Serviço iniciado: {time}", DateTimeOffset.UtcNow);

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception excecao)
            {
                Log.Fatal(excecao, "Falha geral");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()               
                .ConfigureServices((hostContext, services) =>
                {
                    var applicationConfig = new ApplicationConfig();

                    hostContext.Configuration.Bind(nameof(ApplicationConfig), applicationConfig);

                    services.AddSingleton(applicationConfig);

                    services.AddSingleton<IEscritorArquivo, EscritorArquivo>();
                    services.AddSingleton<IEscritorJsonArquivo, EscritorJsonArquivo>();
                    services.AddSingleton<ILeitorArquivo, LeitorArquivo>();

                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
        }
    }
}
