using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartBackend.Hubs;
using ChartBackend.Models;
using ChartBackend.Subscription;
using ChartBackend.Subscription.Middleware;

namespace ChartBackend
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options=>options.AddDefaultPolicy(policy =>
            {
                policy.AllowCredentials().AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(x => true);
            }));
            services.AddSignalR();
            services.AddSingleton<DatabaseSubscription<Employee>>();
            services.AddSingleton<DatabaseSubscription<Sale>>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseDatabaseSubscription<DatabaseSubscription<Sale>>("Sales");
            app.UseDatabaseSubscription<DatabaseSubscription<Employee>>("Employees");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChartHub>("/charthub");
            });
        }
    }
}
