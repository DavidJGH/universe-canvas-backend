#nullable enable
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScottBrady91.AspNetCore.Identity;
using universe_canvas.Hubs;
using universe_canvas.Models;
using universe_canvas.Repositories;
using universe_canvas.Services;

namespace universe_canvas
{
    public class Startup
    {
        private ITimerService? _timerService;
        private ICanvasRepository? _canvasRepository;
        private ICanvasService? _canvasService;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    var key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
            
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
            services.Configure<DatabaseSettings>(
                Configuration.GetSection("Database"));
            
            services.AddSingleton<ICanvasService, CanvasService>();
            services.AddSingleton<ITimerService, TimerService>();
            services.AddSingleton<IPasswordHasher<User>, BCryptPasswordHasher<User>>();
            
            // repositories
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ICanvasRepository, CanvasRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment env, IHubContext<CanvasHub> hub, ITimerService timerService, ICanvasRepository canvasRepository, ICanvasService canvasService)
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CanvasHub>("/canvasHub");
            });
            
            _timerService = timerService;
            _canvasRepository = canvasRepository;
            _canvasService = canvasService;
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            
            // service startup
            Task.Run(async () =>
            {
                Canvas? canvas = await canvasRepository!.GetAsync();
                if (canvas != null)
                {
                    canvasService.Canvas = canvas;
                }
                else
                {
                    await canvasRepository.InsertAsync(canvasService.Canvas);
                    canvasService.Canvas.Id = (await canvasRepository.GetAsync())?.Id;
                }
                timerService.AddTimer(60000, () => hub.Clients.All.SendAsync("TransferCompleteCanvas", canvasService.Canvas));
                timerService.AddTimer(10 * 60000, () =>
                {
                    Task.Run(async () => await canvasRepository.ReplaceAsync(canvasService.Canvas)).Wait();
                });
                timerService.AddTimer(500, () =>
                {
                    hub.Clients.All.SendAsync("TransferCanvasChanges", canvasService.CanvasChanges);
                    canvasService.CanvasChanges.Clear();
                });
            }).Wait();
        }
        
        private void OnShutdown()
        {
            _timerService?.StopAllTimers();
            Task.Run(async () =>
            {
                if (_canvasRepository != null && _canvasService != null) await _canvasRepository.ReplaceAsync(_canvasService.Canvas);
            }).Wait();
        }
    }
}