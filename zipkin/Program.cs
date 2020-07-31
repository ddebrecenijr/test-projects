using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace zipkin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetLogger(nameof(Program));
            try {
                logger.Info("Starting Test API");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex) {
                logger.Error(ex, "Stopped program due to unexpected exception");
                throw;
            }
            finally {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                })
                .UseKestrel(options => {
                    options.AllowSynchronousIO = true;
                    options.Listen(IPAddress.Any, 5001);
                })
                .UseNLog();
    }
}
