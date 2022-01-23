using System.IO;
using CsvUploader.Utils.ErrorHandling;
using CSVUploaderAPI.Bus;
using CSVUploaderAPI.Config;
using CSVUploaderAPI.Contract;
using CSVUploaderAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using SlimMessageBus.Host.Memory;
using SlimMessageBus.Host.MsDependencyInjection;

namespace CSVUploaderAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CSVUploaderAPI", Version = "v1" });
            });
            services.Configure<UploadFileConfig>(Configuration.GetSection("UploadFileConfig"))
                .Configure<JsonOutputConfig>(Configuration.GetSection("JsonOutputConfig"))
                .AddSingleton<UploadFileService>()
                .AddTransient<ImportCsvFileService>()
                .AddTransient<ImportToMongoDbHandler>()
                .AddTransient(typeof(IEventDispatcher<>),typeof(EventDispatcher<>))
                .AddTransient<IDbRepository, MongoDbClothesRepository>()
                .AddTransient<IFileRepository, JsonFileRepository>()
                .AddSingleton<IMongoClient>(new MongoClient(Configuration["ConnectionStrings:ClothesDB"]))
                .AddSlimMessageBus((builder, svp) =>
                {
                    builder
                        .Produce<FileUploadedEvent>(x => x.DefaultTopic("UploadedFile-Topic"))
                        .Consume<FileUploadedEvent>(x =>
                            x.Topic("UploadedFile-Topic").WithConsumer<ImportCsvFileService>())
                        .Produce<CsvRecordParsedEvent>(x => x.DefaultTopic("CsvRecordParsed-Topic"))
                        .Consume<CsvRecordParsedEvent>(x =>
                            x.Topic("CsvRecordParsed-Topic").WithConsumer<ImportToMongoDbHandler>())
                        .WithDependencyResolver(new MsDependencyInjectionDependencyResolver(svp))
                        .WithProviderMemory(new MemoryMessageBusSettings
                        {
                            EnableMessageSerialization = false
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> log, IConfiguration config)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSVUploader v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCustomErrorHandler(log, "CSVUploaderAPI", env.IsDevelopment());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            CreateRequiredFolders(config);
        }

        public void CreateRequiredFolders(IConfiguration config)
        {
            var storedFilesPath = config.GetValue<string>("UploadFileConfig:StoredFilesPath");
            CreateDirectoryIfNotExists(storedFilesPath);

            var jsonOutputDirectory = config.GetValue<string>("JsonOutputConfig:JsonOutputDirectory");
            CreateDirectoryIfNotExists(jsonOutputDirectory);

        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
