using System;
using GifProxy.Api.Extensions;
using GifProxy.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GifProxy.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<InitializationService>();
            services.AddSingleton<GiphyClient>();
            services.AddRouting();
            services.AddControllers().AddNewtonsoftJson();
            services.ConfigureLiteDb();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(builder => builder.MapControllers());
        }
    }
}