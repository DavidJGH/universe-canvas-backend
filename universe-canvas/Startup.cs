#nullable enable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using universe_canvas.Hubs;
using universe_canvas.Models;
using universe_canvas.Services;

namespace universe_canvas
{
    public class Startup
    {
        private TimerService? _timerService;
        private CanvasService? _canvasService;
        
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "universe_canvas", Version = "v1" });
            });
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost:4200")
                    .WithOrigins("https://universe-canvas.web.app")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddSingleton<TimerService>();
            services.Configure<DatabaseSettings>(
                Configuration.GetSection("Database"));
            services.AddSingleton<CanvasService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment env, IHubContext<CanvasHub> hub, TimerService? timerService, CanvasService? canvasService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "universe_canvas v1"));
            }

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CanvasHub>("/canvasHub");
            });
            
            _timerService = timerService;
            _canvasService = canvasService;
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            
            // service startup
            Task.Run(async () =>
            {
                Canvas? canvas = await canvasService!.GetAsync();
                if (canvas != null)
                {
                    CanvasHub.Canvas = canvas;
                }
                else
                {
                    await canvasService.InsertAsync(CanvasHub.Canvas);
                    CanvasHub.Canvas.Id = (await canvasService.GetAsync())?.Id;
                }
                _timerService!.AddTimer(60000, () => hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas));
                _timerService.AddTimer(10 * 60000, () =>
                {
                    Task.Run(async () => await canvasService.ReplaceAsync(CanvasHub.Canvas)).Wait();
                });
                _timerService.AddTimer(500, () =>
                {
                    hub.Clients.All.SendAsync("TransferCanvasChanges", CanvasHub.CanvasChanges);
                    CanvasHub.ClearChanges();
                });
            }).Wait();
        }
        
        private void OnShutdown()
        {
            _timerService?.StopAllTimers();
            Task.Run(async () =>
            {
                if (_canvasService != null) await _canvasService.ReplaceAsync(CanvasHub.Canvas);
            }).Wait();
        }
    }
}