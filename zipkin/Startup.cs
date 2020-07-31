using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;
using zipkin4net.Middleware;
using Microsoft.EntityFrameworkCore;
using zipkin.Models;


namespace zipkin
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
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            appLifetime.ApplicationStarted.Register(() => {
                TraceManager.SamplingRate = 1.0f;
                
                var httpSender = new HttpZipkinSender("http://localhost:9411", "application/json");
                var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());

                TraceManager.RegisterTracer(tracer);
                TraceManager.Start(new TracingLogger(loggerFactory, nameof(Startup)));
            });

            appLifetime.ApplicationStopped.Register(() => TraceManager.Stop());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Does not break here
            app.UseTracing("test-web-app");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Breaks if here
            // app.UseTracing("test-web-app");
        }
    }
}
