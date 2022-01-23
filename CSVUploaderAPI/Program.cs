using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace CSVUploaderAPI
{
    public class Program
    {
        static Logger _logger;

        public static void Main(string[] args)
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                _logger.Info("init new instance from CSVUploader API");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Stopped CSVUploader API because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.Limits.MaxRequestBodySize = long.MaxValue;
                        });

                }).UseNLog();
    }
}
