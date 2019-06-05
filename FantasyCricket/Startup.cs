using FantasyCricket.Common.Filters;
using FantasyCricket.Database;
using FantasyCricket.Service;
using FantasyCricket.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace FantasyCricket
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


            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));


            services.AddMvc(config =>
            {
                config.Filters.Add(typeof(ExceptionFilter));
            })
        .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());


            services.AddSignalR();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<ILiveScore, CricApiLiveScore>();
            services.AddSingleton<IPlayerInfo, PlayerInfo>();
            services.AddSingleton<ISeriesInfo, SeriesInfo>();
            services.AddSingleton<IUser, User>();
            DatabaseSetup databaseSetup = new DatabaseSetup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseFileServer();
            app.UseCors("MyPolicy");
            app.UseMvc();

            app.UseSignalR(routes =>
            {
      routes.MapHub<LiveScoreHub>("/livescore");
            });

        }
    }
}
